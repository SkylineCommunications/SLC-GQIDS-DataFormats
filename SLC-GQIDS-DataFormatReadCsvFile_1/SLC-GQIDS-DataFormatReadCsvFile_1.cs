namespace SLCGQIDSDataFormatReadCsvFile_1
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using CsvHelper;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;

    using GQIHelper.HeaderInfo;

    using Skyline.DataMiner.Analytics.GenericInterface;
    using Skyline.DataMiner.Utils.SecureCoding.SecureIO;

    [GQIMetaData(Name = "CSV File")]
    public sealed class SLCGQIDSDataFormatReadCsvFile : IGQIDataSource, IGQIInputArguments, IGQIOnInit, IGQIUpdateable
    {
        private const string CSV_ROOT_PATH = @"C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadCsvFile";

        private readonly string[] commonDelimeters = { ",", ";", "\t", "|" };
        private readonly GQIStringArgument fileName = new GQIStringArgument("File name") { IsRequired = true };
        private readonly GQIStringArgument delimiter = new GQIStringArgument("Delimiter") { IsRequired = false };
        private readonly GQIStringDropdownArgument headerCapitalization = new GQIStringDropdownArgument("Header capitalization", new[] { "Original", "Uppercase", "Titlecase", "Lowercase" }) { IsRequired = false, DefaultValue = "Original" };

        private readonly DateTimeConverter _dateTimeConverter = new DateTimeConverter();

        private GQIDMS _dms;

        private IGQILogger _logger;
        private string _fileName;
        private string _delimiter;
        private string _headerCapitalization;
        private string _csvFilePath;
        private HeaderInfo _headerInfo;
        private GQIRow[] _rows;
        private int _rowCount;
        private IGQIUpdater _updater;
        private FileSystemWatcher _watcher;

        public OnInitOutputArgs OnInit(OnInitInputArgs args)
        {
            _dms = args.DMS;
            _logger = args.Logger;
            return new OnInitOutputArgs();
        }

        public GQIArgument[] GetInputArguments()
        {
            return new GQIArgument[] { fileName, delimiter, headerCapitalization };
        }

        public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
        {
            _headerInfo = default;
            _rows = null;

            var securePath = SecurePath.CreateSecurePath(CSV_ROOT_PATH);
            if (!Directory.Exists(securePath))
            {
                Directory.CreateDirectory(securePath);
            }

            _fileName = args.GetArgumentValue(fileName);

            if (!_fileName.Contains(".csv"))
            {
                _fileName = $"{_fileName}.csv";
            }

            _csvFilePath = SecurePath.ConstructSecurePath(CSV_ROOT_PATH, _fileName);

            if (!File.Exists(_csvFilePath))
                throw new GenIfException($"Csv file does not exist: {_csvFilePath}");

            _delimiter = args.GetArgumentValue(delimiter);
            _headerCapitalization = args.GetArgumentValue(headerCapitalization);

            ReadCSVFile();

            return new OnArgumentsProcessedOutputArgs();
        }

        public GQIColumn[] GetColumns()
        {
            return _headerInfo.Columns;
        }

        public GQIPage GetNextPage(GetNextPageInputArgs args)
        {
            return new GQIPage(_rows);
        }

        public void OnStartUpdates(IGQIUpdater updater)
        {
            _updater = updater;

            var directory = Path.GetDirectoryName(_csvFilePath);
            var fileName = Path.GetFileName(_csvFilePath);
            _watcher = new FileSystemWatcher(directory, fileName) { NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime, EnableRaisingEvents = true };

            _watcher.Changed += OnChanged;
        }

        public void OnStopUpdates()
        {
            if (_watcher is null)
                return;

            _watcher.Changed -= OnChanged;
            _watcher.Dispose();
            _updater = null;
        }

        private void ReadCSVFile()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = DetectDelimiter(),
            };

            using (var fileStream = new FileStream(_csvFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var streamReader = new StreamReader(fileStream))
            using (var csvReader = new CsvReader(streamReader, config))
            {
                csvReader.Read();
                csvReader.ReadHeader();
                _headerInfo = HeaderInfo.GetHeaderInfo(csvReader.HeaderRecord, _headerCapitalization);
                _rows = ReadRows(csvReader);
                _rowCount = _rows.Length;
            }
        }

        private string DetectDelimiter()
        {
            if (!String.IsNullOrEmpty(_delimiter))
            {
                return _delimiter;
            }

            Dictionary<string, int> delimiterCounts = new Dictionary<string, int>();

            using (var fileStream = new FileStream(_csvFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var streamReader = new StreamReader(fileStream))
            {
                string headerLine = streamReader.ReadLine();

                foreach (var delimiter in commonDelimeters)
                {
                    delimiterCounts[delimiter] = headerLine.Split(new[] { delimiter }, StringSplitOptions.None).Length - 1;
                }

                var detectedDelimiter = delimiterCounts.OrderByDescending(pair => pair.Value).FirstOrDefault();

                if (detectedDelimiter.Value > 0)
                {
                    return detectedDelimiter.Key;
                }

                throw new GenIfException($"No Delimiter detected, please specify a delimiter.");
            }
        }

        private GQIRow[] ReadRows(CsvReader csvReader)
        {
            var columnTypes = _headerInfo.GetColumnTypes();
            return ReadRows(csvReader, columnTypes);
        }

        private GQIRow[] ReadRows(CsvReader reader, Type[] columnTypes)
        {
            var rows = new List<GQIRow>();
            while (reader.Read())
            {
                var row = ReadRow(reader, columnTypes, rows.Count);
                rows.Add(row);
            }

            return rows.ToArray();
        }

        private GQIRow ReadRow(CsvReader reader, Type[] columnTypes, int key)
        {
            var cells = columnTypes.Select((type, index) => GetCell(reader, index, type));
            return new GQIRow(key.ToString(), cells.ToArray());
        }

        private GQIRow ReadRow(CsvReader reader, int keyIndex, Type[] columnTypes)
        {
            var key = reader.GetField(keyIndex);
            var cells = columnTypes.Select((type, index) => GetCell(reader, index, type));
            return new GQIRow(key, cells.ToArray());
        }

        private GQICell GetCell(CsvReader reader, int index, Type type)
        {
            if (type == typeof(DateTime))
            {
                var dateTime = reader.GetField<DateTime>(index, _dateTimeConverter);
                return new GQICell() { Value = dateTime };
            }

            var value = reader.GetField(type, index);
            return new GQICell() { Value = value };
        }

        private void UpdateRows()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = DetectDelimiter(),
            };

            using (var fileStream = new FileStream(_csvFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (var streamReader = new StreamReader(fileStream))
            using (var csvReader = new CsvReader(streamReader, config))
            {
                csvReader.Read();
                var columnTypes = _headerInfo.GetColumnTypes();
                UpdateRows(csvReader, columnTypes);
            }
        }

        private void UpdateRows(CsvReader reader, Type[] columnTypes)
        {
            var updatedRows = new List<GQIRow>();
            int index = 0;

            while (reader.Read())
            {
                var row = ReadRow(reader, columnTypes, index);

                var matchingRow = _rows.FirstOrDefault(x => x.Key == row.Key);

                if (matchingRow != null)
                {
                    _updater.UpdateRow(row);
                }
                else
                {
                    _updater.AddRow(row);
                }

                updatedRows.Add(row);
                index++;
            }

            RemoveExtraRows(updatedRows);
        }

        private void RemoveExtraRows(List<GQIRow> updatedRows)
        {
            var previousRowKeys = _rows.Select(x => x.Key).ToList();
            var updatedRowsKeys = updatedRows.Select(x => x.Key).ToList();

            var keysToRemove = previousRowKeys.Except(updatedRowsKeys);
            foreach (var rowKey in keysToRemove)
            {
                _updater.RemoveRow(rowKey);
            }

            _rows = updatedRows.ToArray();
            _rowCount = _rows.Length;
        }

        private void OnChanged(object sender, FileSystemEventArgs args)
        {
            try
            {
                UpdateRows();
            }
            catch (Exception ex)
            {
                throw new GenIfException($"Failed to update rows: {ex.Message}");
            }
        }

        private class DateTimeConverter : ITypeConverter
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
