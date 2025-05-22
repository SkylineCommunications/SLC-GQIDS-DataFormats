namespace JSONFile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Common.HeaderInfo;
    using Common.RealTimeUpdates;

    using Newtonsoft.Json;

    using Skyline.DataMiner.Analytics.GenericInterface;

    [GQIMetaData(Name = "JSON File")]
    public class JSONFile : IGQIDataSource, IGQIInputArguments, IGQIOnInit, IGQIOnPrepareFetch, IGQIOnDestroy/*, IGQIUpdateable*/
	{
        private const string JSON_ROOT_PATH = @"C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadJsonFile\";

        private readonly GQIStringArgument fileName = new GQIStringArgument("File name") { IsRequired = true };
        private readonly GQIStringDropdownArgument headerCapitalization =
        new GQIStringDropdownArgument(
            "Header capitalization",
            new[] { "Original", "Uppercase", "Titlecase", "Lowercase" })
        {
            IsRequired = false,
            DefaultValue = "Original",
        };

        private readonly object _lock = new object();
        private string _headerCapitalization;
        private List<GQIColumn> _columns = new List<GQIColumn>();
        private List<GQIRow> _currentRows = new List<GQIRow>();
        private string _jsonFilePath;
        private IGQIUpdater _updater;
        private FileSystemWatcher _watcher;
        private GQIPageEnumerator pageEnumerator;
        private DateTime _lastReadTime = DateTime.MinValue;
        private IGQILogger _logger;

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
            if (!Directory.Exists(JSON_ROOT_PATH))
            {
                Directory.CreateDirectory(JSON_ROOT_PATH);
            }

            var fileNameValue = args.GetArgumentValue(fileName);

            if (!fileNameValue.Contains(".json"))
            {
                fileNameValue = $"{fileNameValue}.json";
            }

            _jsonFilePath = Path.Combine(JSON_ROOT_PATH, fileNameValue);

            if (!File.Exists(_jsonFilePath))
                throw new GenIfException($"Json file does not exist: {_jsonFilePath}");

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
            var directory = Path.GetDirectoryName(_jsonFilePath);
            var jsonFileName = Path.GetFileName(_jsonFilePath);
            _watcher = new FileSystemWatcher(directory, jsonFileName) { NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime, EnableRaisingEvents = true };

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
            var rows = new List<GQIRow>();

            string jsonContent;
            using (var stream = new FileStream(_jsonFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                jsonContent = reader.ReadToEnd();
            }

            var jsonData = JsonConvert.DeserializeObject<JSONData>(jsonContent);

            if (jsonData?.Columns == null || jsonData.Rows == null)
            {
                throw new GenIfException("Invalid JSON structure.");
            }

            // Process Columns
            _columns = jsonData.Columns.Select(column => CreateColumn(column.Name, column.Type)).ToList();

            // Process Rows
            var rowKey = 0;
            foreach (var row in jsonData.Rows)
            {
                var cells = new List<GQICell>();
                for (int i = 0; i < _columns.Count; i++)
                {
                    var cell = row.Cells[i];
                    cells.Add(new GQICell
                    {
                        Value = ParseValue(cell.Value, _columns[i]),
                        DisplayValue = cell.DisplayValue,
                    });
                }

                rows.Add(new GQIRow($"{rowKey}", cells.ToArray()));
                rowKey++;
            }

            return rows;
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

#pragma warning disable S101 // Types should be named in PascalCase
        public class JSONData
        {
            public JSONColumn[] Columns { get; set; }

            public JSONRow[] Rows { get; set; }

            public class JSONColumn
            {
                public string Name { get; set; }

                public string Type { get; set; }
            }

            public class JSONRow
            {
                public JSONCell[] Cells { get; set; }
            }

            public class JSONCell
            {
                public object Value { get; set; }

                public string DisplayValue { get; set; }
            }
        }
#pragma warning restore S101 // Types should be named in PascalCase
    }
}
