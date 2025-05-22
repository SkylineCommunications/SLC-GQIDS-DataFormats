/*
****************************************************************************
*  Copyright (c) 2025,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

dd/mm/2025	1.0.0.1		XXX, Skyline	Initial version
****************************************************************************
*/

namespace SLC_GQIDS_DataFormatReadXmlFile_1
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Xml.Linq;

	using Common.HeaderInfo;
	using Common.RealTimeUpdates;

	using Skyline.DataMiner.Analytics.GenericInterface;

	[GQIMetaData(Name = "XML File")]
	public class XmlFile : IGQIDataSource, IGQIInputArguments, IGQIOnDestroy, IGQIOnPrepareFetch, IGQIOnInit/*, IGQIUpdateable*/
	{
		private const string XML_ROOT_PATH = @"C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadXMLFile\";

		private readonly GQIStringArgument fileName = new GQIStringArgument("File name") { IsRequired = true };
		private readonly GQIStringDropdownArgument headerCapitalization =
		new GQIStringDropdownArgument(
			"Header capitalization",
			new[] { "Original", "Uppercase", "Titlecase", "Lowercase", })
		{
			IsRequired = false,
			DefaultValue = "Original",
		};

		private readonly object _lock = new object();
		private string _headerCapitalization;
		private List<GQIColumn> _columns = new List<GQIColumn>();
		private List<GQIRow> _currentRows = new List<GQIRow>();
		private string _xmlFilePath;
		private IGQIUpdater _updater;
		private FileSystemWatcher _watcher;
		private GQIPageEnumerator pageEnumerator;
		private DateTime _lastReadTime = DateTime.MinValue;
		private IGQILogger _logger;

		public enum HeaderCapitalization
		{
			Uppercase,
			Lowercase,
			Titlecase,
			Original,
		}

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			_logger = args.Logger;
			return new OnInitOutputArgs();
		}

		public GQIArgument[] GetInputArguments()
		{
			return new GQIArgument[] { fileName, headerCapitalization };
		}

		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			_updater = null;

			_headerCapitalization = args.GetArgumentValue(headerCapitalization);
			if (!Directory.Exists(XML_ROOT_PATH))
			{
				Directory.CreateDirectory(XML_ROOT_PATH);
			}

			var fileNameValue = args.GetArgumentValue(fileName);

			if (!fileNameValue.Contains(".xml"))
			{
				fileNameValue = $"{fileNameValue}.xml";
			}

			_xmlFilePath = Path.Combine(XML_ROOT_PATH, fileNameValue);

			if (!File.Exists(_xmlFilePath))
				throw new GenIfException($"XML file does not exist: {_xmlFilePath}");

			_currentRows = GetNewRows();
			return new OnArgumentsProcessedOutputArgs();
		}

		public GQIColumn[] GetColumns()
		{
			return _columns.ToArray();
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			return pageEnumerator.GetNextPage(500);
		}

		public OnPrepareFetchOutputArgs OnPrepareFetch(OnPrepareFetchInputArgs args)
		{
			var directory = Path.GetDirectoryName(_xmlFilePath);
			var xmlFileName = Path.GetFileName(_xmlFilePath);
			_watcher = new FileSystemWatcher(directory, xmlFileName) { NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime, EnableRaisingEvents = true };

			pageEnumerator = new GQIPageEnumerator(_currentRows);
			return new OnPrepareFetchOutputArgs();
		}

		public void OnStartUpdates(IGQIUpdater updater)
		{
			_updater = updater;

			_watcher.Changed += OnChanged;
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

		private static object ParseValue(object value, GQIColumn column)
		{
			switch (column.GetType().Name)
			{
				case nameof(GQIIntColumn):
					return Convert.ToInt32(value);

				case nameof(GQIStringColumn):
					return value?.ToString() ?? string.Empty;

				case nameof(GQIDateTimeColumn):
					return DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(value)).UtcDateTime;

				case nameof(GQIDoubleColumn):
					return Convert.ToDouble(value);

				case nameof(GQIBooleanColumn):
					return Convert.ToBoolean(value);

				default:
					return value?.ToString() ?? string.Empty;
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

				List<GQIRow> newRows = GetNewRows();
				try
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

		private List<GQIRow> GetNewRows()
		{
			XDocument xmlDocument;
			using (var stream = new FileStream(_xmlFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				xmlDocument = XDocument.Load(stream);
			}

			var rootElement = xmlDocument.Root;

			if (rootElement == null)
			{
				throw new GenIfException("Invalid XML structure: Missing root element.");
			}

			// Process Columns
			_columns = rootElement.Element("Columns")?.Elements("Column")
								   .Select(column => CreateColumn((string)column.Element("Name"), (string)column.Element("Type")))
								   .ToList() ?? new List<GQIColumn>();

			// Process Rows
			var rowsElement = rootElement.Element("Rows")?.Elements("Row").ToList() ?? new List<XElement>();
			return ProcessRows(rowsElement);
		}

		private List<GQIRow> ProcessRows(List<XElement> rowElements)
		{
			var processedRows = new List<GQIRow>();
			for (var index = 0; index < rowElements.Count; index++)
			{
				var row = rowElements[index];
				var cells = ProcessCells(row.Element("Cells")?.Elements("Cell").ToList() ?? new List<XElement>());
				processedRows.Add(new GQIRow(index.ToString(), cells.ToArray()));
			}

			return processedRows;
		}

		private List<GQICell> ProcessCells(List<XElement> cellElements)
		{
			var processedCells = new List<GQICell>();
			for (var cellIndex = 0; cellIndex < cellElements.Count; cellIndex++)
			{
				var cell = cellElements[cellIndex];
				var value = ParseValue(cell.Element("Value")?.Value, _columns[cellIndex]);
				var displayValue = cell.Element("DisplayValue")?.Value ?? cell.Element("Value")?.Value;
				processedCells.Add(new GQICell { Value = value, DisplayValue = displayValue });
			}

			return processedCells;
		}

		private GQIColumn CreateColumn(string name, string type)
		{
			string headerCapitalizedName = string.Empty;
			if (!Enum.TryParse(_headerCapitalization, true, out HeaderInfo.HeaderCapitalization headerEnum))
			{
				headerCapitalizedName = name;
			}
			else
			{
				headerCapitalizedName = HeaderInfo.GetHeaderCapitalization(name, headerEnum);
			}

			switch (type.ToLower())
			{
				case "string":
					return new GQIStringColumn(headerCapitalizedName);
				case "int":
					return new GQIIntColumn(headerCapitalizedName);
				case "datetime":
					return new GQIDateTimeColumn(headerCapitalizedName);
				case "double":
					return new GQIDoubleColumn(headerCapitalizedName);
				case "boolean":
					return new GQIBooleanColumn(headerCapitalizedName);
				default:
					return new GQIStringColumn(headerCapitalizedName);
			}
		}
	}
}
