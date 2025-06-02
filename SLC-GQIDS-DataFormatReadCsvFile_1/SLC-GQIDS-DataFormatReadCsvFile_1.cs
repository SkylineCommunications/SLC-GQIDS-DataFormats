namespace SLCGQIDSDataFormatReadCsvFile_1
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;

	using Common.HeaderInfo;
	using Common.RealTimeUpdates;

	using CsvHelper;
	using CsvHelper.Configuration;
	using CsvHelper.TypeConversion;

	using Skyline.DataMiner.Analytics.GenericInterface;

	[GQIMetaData(Name = "CSV File")]
	public class SLCGQIDSDataFormatReadCsvFile : IGQIDataSource, IGQIOnPrepareFetch, IGQIInputArguments, IGQIOnInit, IGQIOnDestroy/*, IGQIUpdateable*/
	{
		private const string CSV_ROOT_PATH = @"C:\Skyline DataMiner\Documents\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadCsvFile";

		private readonly string[] commonDelimeters = { ",", ";", "\t", "|" };
		private readonly GQIStringArgument fileName = new GQIStringArgument("File name") { IsRequired = true };
		private readonly GQIStringArgument delimiter = new GQIStringArgument("Delimiter") { IsRequired = false };
		private readonly GQIStringDropdownArgument headerCapitalization =
			new GQIStringDropdownArgument(
				"Header capitalization",
				new[] { "Original", "Uppercase", "Titlecase", "Lowercase" })
			{
				IsRequired = false,
				DefaultValue = "Original",
			};

		private readonly DateTimeConverter _dateTimeConverter = new DateTimeConverter();

		private readonly object _lock = new object();
		private string _delimiter;
		private string _headerCapitalization;
		private string _csvFilePath;
		private HeaderInfo _headerInfo;
		private List<GQIRow> _currentRows;
		private IGQIUpdater _updater;
		private FileSystemWatcher _watcher;
		private DateTime _lastReadTime = DateTime.MinValue;
		private IGQILogger _logger;
		private GQIPageEnumerator pageEnumerator;

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			_logger = args.Logger;
			return new OnInitOutputArgs();
		}

		public GQIArgument[] GetInputArguments()
		{
			return new GQIArgument[] { fileName, delimiter, headerCapitalization };
		}

		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			_headerInfo = null;
			_currentRows = null;
			if (!Directory.Exists(CSV_ROOT_PATH))
			{
				Directory.CreateDirectory(CSV_ROOT_PATH);
			}

			var fileNameValue = args.GetArgumentValue(fileName);

			if (!fileNameValue.Contains(".csv"))
			{
				fileNameValue = $"{fileNameValue}.csv";
			}

			_csvFilePath = Path.Combine(CSV_ROOT_PATH, fileNameValue);

			if (!File.Exists(_csvFilePath))
				throw new GenIfException($"Csv file does not exist: {_csvFilePath}");

			_delimiter = args.GetArgumentValue(delimiter);
			_headerCapitalization = args.GetArgumentValue(headerCapitalization);

			_currentRows = ReadCsvFile();

			return new OnArgumentsProcessedOutputArgs();
		}

		public GQIColumn[] GetColumns()
		{
			return _headerInfo.Columns;
		}

		public OnPrepareFetchOutputArgs OnPrepareFetch(OnPrepareFetchInputArgs args)
		{
			var directory = Path.GetDirectoryName(_csvFilePath);
			var sFileName = Path.GetFileName(_csvFilePath);
			_watcher = new FileSystemWatcher(directory, sFileName) { NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime, EnableRaisingEvents = true };

			pageEnumerator = new GQIPageEnumerator(_currentRows);
			return new OnPrepareFetchOutputArgs();
		}

		public void OnStartUpdates(IGQIUpdater updater)
		{
			_updater = updater;
			_watcher.Changed += OnChanged;
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			return pageEnumerator.GetNextPage(500);
		}

		public void OnStopUpdates()
		{
			_watcher.Changed -= OnChanged;
			_updater = null;
		}

		public OnDestroyOutputArgs OnDestroy(OnDestroyInputArgs args)
		{
			_watcher?.Dispose();
			return new OnDestroyOutputArgs();
		}

		private List<GQIRow> ReadCsvFile()
		{
			using (var fileStream = new FileStream(_csvFilePath, FileMode.Open, FileAccess.Read))
			using (var streamReader = new StreamReader(fileStream))
			{
				string headerLine = streamReader.ReadLine();
				_delimiter = DetectDelimiter(headerLine);
				var config = new CsvConfiguration(CultureInfo.InvariantCulture)
				{
					Delimiter = _delimiter,
				};

				streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
				streamReader.DiscardBufferedData();

				using (var csvReader = new CsvReader(streamReader, config))
				{
					csvReader.Read();
					csvReader.ReadHeader();
					_headerInfo = HeaderInfo.GetHeaderInfo(csvReader.HeaderRecord, _headerCapitalization);
					return ReadRows(csvReader);
				}
			}
		}

		private string DetectDelimiter(string headerLine)
		{
			if (!string.IsNullOrEmpty(_delimiter))
			{
				return _delimiter;
			}

			Dictionary<string, int> delimiterCounts = new Dictionary<string, int>();

			foreach (var delimiterItem in commonDelimeters)
			{
				delimiterCounts[delimiterItem] = headerLine.Split(new[] { delimiterItem }, StringSplitOptions.None).Length - 1;
			}

			var detectedDelimiter = delimiterCounts.OrderByDescending(pair => pair.Value).FirstOrDefault();

			if (detectedDelimiter.Value > 0)
			{
				return detectedDelimiter.Key;
			}

			throw new GenIfException("No delimiter detected, please specify a delimiter.");
		}

		private List<GQIRow> ReadRows(CsvReader csvReader)
		{
			var columnTypes = _headerInfo.GetColumnTypes();
			return ReadRows(csvReader, columnTypes);
		}

		private List<GQIRow> ReadRows(CsvReader reader, Type[] columnTypes)
		{
			var rows = new List<GQIRow>();
			while (reader.Read())
			{
				var row = ReadRow(reader, columnTypes, rows.Count);
				rows.Add(row);
			}

			return rows;
		}

		private GQIRow ReadRow(CsvReader reader, Type[] columnTypes, int key)
		{
			var cells = columnTypes.Select((type, index) => GetCell(reader, index, type));
			return new GQIRow(key.ToString(), cells.ToArray());
		}

		private GQICell GetCell(CsvReader reader, int index, Type type)
		{
			if (type == typeof(DateTime))
			{
				var dateTime = reader.GetField<DateTime>(index, _dateTimeConverter);
				return new GQICell { Value = dateTime };
			}

			var value = reader.GetField(type, index);
			return new GQICell { Value = value };
		}

		private List<GQIRow> CalculateNewRows()
		{
			try
			{
				using (var fileStream = new FileStream(_csvFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					using (var streamReader = new StreamReader(fileStream))
					{
						var config = new CsvConfiguration(CultureInfo.InvariantCulture)
						{
							Delimiter = _delimiter,
						};

						using (var csvReader = new CsvReader(streamReader, config))
						{
							csvReader.Read();
							csvReader.ReadHeader();
							var newRows = ReadRows(csvReader);
							return newRows;
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Information($"Error reading csv file: {ex.Message}");
				return new List<GQIRow>();
			}
		}

		private void OnChanged(object sender, FileSystemEventArgs args)
		{
			lock (_lock)
			{
				DateTime lastWriteTime;
				lastWriteTime = File.GetLastWriteTime(args.FullPath);
				if ((lastWriteTime - _lastReadTime).TotalMilliseconds < 500)
				{
					return;
				}

				_lastReadTime = lastWriteTime;

				List<GQIRow> newRows = CalculateNewRows();

				try
				{
					if (newRows != null)
					{
						var comparison = new GqiTableComparer(_currentRows, newRows);
						foreach (var row in comparison.RemovedRows)
						{
							_updater.RemoveRow(row.Key);
						}

						foreach (var row in comparison.UpdatedRows)
						{
							_updater.UpdateRow(row);
						}

						foreach (var row in comparison.AddedRows)
						{
							_updater.AddRow(row);
						}
					}
				}
				catch (Exception ex)
				{
					_logger.Error($"Error processing file changes: {ex.Message}");
				}
				finally
				{
					_currentRows = newRows;
				}
			}
		}

		private sealed class DateTimeConverter : ITypeConverter
		{
			public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
			{
				try
				{
					return DateTime.SpecifyKind(DateTime.Parse(text), DateTimeKind.Utc);
				}
				catch (FormatException)
				{
					throw new GenIfException(text);
				}
			}

			public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
			{
				return value.ToString();
			}
		}
	}
}
