namespace JSONFile
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Skyline.DataMiner.Analytics.GenericInterface;
    using Skyline.DataMiner.Utils.SecureCoding.SecureIO;
    using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;

    [GQIMetaData(Name = "JSON File")]
    public class JsonFile : IGQIDataSource, IGQIInputArguments, IGQIUpdateable
    {
        private const string JSON_ROOT_PATH = @"C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad Hoc Data Sources\SLC-GQIDS-ReadJsonFile\";

        private readonly GQIStringArgument fileName = new GQIStringArgument("File name") { IsRequired = true };
        private readonly GQIStringDropdownArgument headerCapitalization = new GQIStringDropdownArgument("Header capitalization", new[] { "Original", "Uppercase", "Titlecase", "Lowercase" }) { IsRequired = false, DefaultValue = "Original" };

        private string _headerCapitalization;
        private List<GQIColumn> _columns = new List<GQIColumn>();
        private List<GQIRow> _rows = new List<GQIRow>();
        private string _jsonFilePath;
        private IGQIUpdater _updater;
        private FileSystemWatcher _watcher;

        public enum HeaderCapitalization
        {
            Uppercase,
            Lowercase,
            Titlecase,
            Original,
        }

        public GQIArgument[] GetInputArguments()
        {
            return new GQIArgument[] { fileName, headerCapitalization };
        }

        public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
        {
            _updater = null;

            _headerCapitalization = args.GetArgumentValue(headerCapitalization);

            var securePath = SecurePath.CreateSecurePath(JSON_ROOT_PATH);
            if (!Directory.Exists(securePath))
            {
                Directory.CreateDirectory(securePath);
            }

            var fileNameValue = args.GetArgumentValue(fileName);

            if (!fileNameValue.Contains(".json"))
            {
                fileNameValue = $"{fileNameValue}.json";
            }

            _jsonFilePath = SecurePath.ConstructSecurePath(JSON_ROOT_PATH, fileNameValue);

            if (!File.Exists(_jsonFilePath))
                throw new GenIfException($"Json file does not exist: {_jsonFilePath}");

            LoadJsonFile();
            return new OnArgumentsProcessedOutputArgs();
        }

        public GQIColumn[] GetColumns()
        {
            return _columns.ToArray();
        }

        public GQIPage GetNextPage(GetNextPageInputArgs args)
        {
            return new GQIPage(_rows.ToArray()) { HasNextPage = false };
        }

        private void LoadJsonFile()
        {
            string jsonContent = File.ReadAllText(_jsonFilePath);

            var jsonData = SecureNewtonsoftDeserialization.DeserializeObject<JSONData>(jsonContent);

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

                _rows.Add(new GQIRow($"{rowKey}", cells.ToArray()));
                rowKey++;
            }
        }

        private GQIColumn CreateColumn(string name, string type)
        {
            string headerCapitalizedName = string.Empty;
            if (!Enum.TryParse(_headerCapitalization, true, out HeaderCapitalization headerEnum))
            {
                headerCapitalizedName = name;
            }
            else
            {
                headerCapitalizedName = GetHeaderCapitalization(name, headerEnum);
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

        private static string GetHeaderCapitalization(string headerName, HeaderCapitalization headerCapitalizationType)
        {
            switch (headerCapitalizationType)
            {
                case HeaderCapitalization.Uppercase:
                    return headerName.ToUpper();
                case HeaderCapitalization.Lowercase:
                    return headerName.ToLower();
                case HeaderCapitalization.Titlecase:
                    TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
                    return textInfo.ToTitleCase(headerName.ToLower());
                case HeaderCapitalization.Original:
                default:
                    return headerName;
            }
        }

        private object ParseValue(object value, GQIColumn column)
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

        public void OnStartUpdates(IGQIUpdater updater)
        {
            _updater = updater;

            var directory = Path.GetDirectoryName(_jsonFilePath);
            var jsonFileName = Path.GetFileName(_jsonFilePath);
            _watcher = new FileSystemWatcher(directory, jsonFileName) { NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime, EnableRaisingEvents = true };

            _watcher.Changed += OnChanged;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            DeleteRows();

            LoadJsonFile();

            foreach (var row in _rows)
            {
                _updater.AddRow(row);
            }
        }

        private void DeleteRows()
        {
            foreach (var row in _rows)
            {
                _updater.RemoveRow(row.Key);
            }

            _rows.Clear();
        }

        public void OnStopUpdates()
        {
            if (_watcher is null)
                return;

            _watcher.Changed -= OnChanged;
            _watcher.Dispose();
            _updater = null;
        }

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
    }
}
