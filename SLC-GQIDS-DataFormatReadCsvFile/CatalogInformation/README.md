# SLC-GQIDS-DataFormatReadCsvFile

A custom GQI data source to import values from CSV files.

The file to be imported must be placed in the following path:
`C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad-Hoc Data Sources\SLC-GQIDS-DataFormatReadCsvFile`

If the specified path does not exist, the GQI automatically creates it, allowing users to add the file later. The solution also detects the delimiter when none is specified and supports adjusting the capitalization of table headers based on user-provided arguments, making it both flexible and user-friendly.

## Delimiter smart detection
  
If no delimiter is provided, the GQI will detect the most common delimiter character in the header (first line) of the CSV file. The smart detection feature is capable of identifying delimiters only from the following set
- `,`
- `;`
- `\t`
- `|`

## Header capitalization

The GQI allows users to specify the desired header capitalization method as an argument. If no value is provided, the headers will retain their original capitalization as read from the CSV file.

## Type conversion

The columns can be automatically parsed to a specific type by suffixing the column name in the CSV header with `::type`.
Where `type` can be one of the following:

- `bool`
- `datetime`
- `double`
- `int`
- `string` (default)

### Example

```CSV
Timestamp::datetime,Test name,Test cases::int,Duration::double,Success::boolean
06/12/2023 01:00,Cisco CMTS,36,1081.788,false
06/12/2023 01:21,Huawei 5600 5800,4,196.621,true
06/12/2023 01:26,Cisco CBR8,41,1443.027,true
06/12/2023 01:50,Arris E6000,33,989.310,false
06/12/2023 02:08,Casa Systems,12,374.005,true
```