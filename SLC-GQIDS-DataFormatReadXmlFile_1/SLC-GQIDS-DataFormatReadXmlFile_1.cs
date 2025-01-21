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

    using Skyline.DataMiner.Analytics.GenericInterface;
    using Skyline.DataMiner.Utils.SecureCoding.SecureIO;

    [GQIMetaData(Name = "XML File")]
    public class XMLFile : IGQIDataSource, IGQIInputArguments, IGQIUpdateable
    {
        private const string XML_ROOT_PATH = @"C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad Hoc Data Sources\SLC-GQIDS-ReadXMLFile\";

        private readonly GQIStringArgument fileName = new GQIStringArgument("File name") { IsRequired = true };
        private readonly GQIStringDropdownArgument headerCapitalization = new GQIStringDropdownArgument("Header capitalization", new[] { "Original", "Uppercase", "Titlecase", "Lowercase" }) { IsRequired = false, DefaultValue = "Original" };

        private string _headerCapitalization;
        private List<GQIColumn> _columns = new List<GQIColumn>();
        private List<GQIRow> _rows = new List<GQIRow>();
        private string _xmlFilePath;
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
            var securePath = SecurePath.CreateSecurePath(XML_ROOT_PATH);
            if (!Directory.Exists(securePath))
            {
                Directory.CreateDirectory(securePath);
            }

            var fileNameValue = args.GetArgumentValue(fileName);

            if (!fileNameValue.Contains(".xml"))
            {
                fileNameValue = $"{fileNameValue}.xml";
            }

            _xmlFilePath = SecurePath.ConstructSecurePath(XML_ROOT_PATH, fileNameValue);

            if (!File.Exists(_xmlFilePath))
                throw new GenIfException($"XML file does not exist: {_xmlFilePath}");

            LoadXmlFile();
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

        private void LoadXmlFile()
        {
            var xmlDocument = XDocument.Load(_xmlFilePath);
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
            _rows = ProcessRows(rowsElement);
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

            var directory = Path.GetDirectoryName(_xmlFilePath);
            var xmlFileName = Path.GetFileName(_xmlFilePath);
            _watcher = new FileSystemWatcher(directory, xmlFileName) { NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime, EnableRaisingEvents = true };

            _watcher.Changed += OnChanged;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            DeleteRows();

            LoadXmlFile();

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
    }
}
