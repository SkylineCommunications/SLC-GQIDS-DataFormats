namespace GQIHelper.HeaderInfo
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Skyline.DataMiner.Analytics.GenericInterface;

    public class HeaderInfo
    {
        public enum HeaderCapitalization
        {
            Uppercase,
            Lowercase,
            Titlecase,
            Original,
        }

        public HeaderInfo(int keyIndex, GQIColumn[] columns)
        {
            KeyIndex = keyIndex;
            Columns = columns;
        }

        public int KeyIndex { get; }

        public GQIColumn[] Columns { get; }

        public Type[] GetColumnTypes()
        {
            return Columns.Select(column => GetColumnType(column.Type)).ToArray();
        }

        private Type GetColumnType(GQIColumnType type)
        {
            switch (type)
            {
                case GQIColumnType.Boolean: return typeof(bool);
                case GQIColumnType.DateTime: return typeof(DateTime);
                case GQIColumnType.Double: return typeof(double);
                case GQIColumnType.Int: return typeof(int);
                default: return typeof(string);
            }
        }

        public static HeaderInfo GetHeaderInfo(string[] header, string headerCapitalization)
        {
            var keyIndex = -1;
            var columns = new List<GQIColumn>();

            for (int i = 0; i < header.Length; i++)
            {
                var columnInfo = GetColumnInfo(header[i]);

                if (columnInfo.type == "key")
                {
                    if (keyIndex != -1)
                        throw new GenIfException($"Duplicate key definition at column {keyIndex} and column {i}.");
                    keyIndex = i;
                }

                var column = GetColumn(columnInfo.name, columnInfo.type, headerCapitalization);
                columns.Add(column);
            }

            return new HeaderInfo(keyIndex, columns.ToArray());
        }

        private static (string name, string type) GetColumnInfo(string head)
        {
            var separatorIndex = head.IndexOf("::");
            if (separatorIndex == -1)
                return (head, "string");

            var name = head.Substring(0, separatorIndex);
            var type = head.Substring(separatorIndex + 2);
            return (name, type);
        }

        private static GQIColumn GetColumn(string name, string type, string headerCapitalization)
        {
            string headerCapitalizedName = string.Empty;
            if (!Enum.TryParse(headerCapitalization, true, out HeaderCapitalization headerEnum))
            {
                headerCapitalizedName = name;
            }
            else
            {
                headerCapitalizedName = GetHeaderCapitalization(name, headerEnum);
            }

            switch (type)
            {
                case "bool": return new GQIBooleanColumn(headerCapitalizedName);
                case "datetime": return new GQIDateTimeColumn(headerCapitalizedName);
                case "double": return new GQIDoubleColumn(headerCapitalizedName);
                case "int": return new GQIIntColumn(headerCapitalizedName);
                default: return new GQIStringColumn(headerCapitalizedName);
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
    }
}
