# CSV File Reader

## About

This package contains a GQI data source that can read CSV files.

> [!TIP]
> Easily see how to implement a Dashboard/Low Code app with this script with our [Generic File Reader GQI Tutorial](https://catalog.dataminer.services/details/f7ecd365-7bf9-406d-978f-eaca9e3aa9c2).

## Key Features

### Smart delimiter detection
  
If no delimiter is provided, GQI will detect the most common delimiter character in the header (i.e. first line) of the CSV file.

The smart detection feature is capable of identifying delimiters only from the following set:

- `,`
- `;`
- `\t`
- `|`

## Header capitalization

GQI allows users to specify the desired header capitalization method as an argument.

If no value is provided, the headers will retain their original capitalization as read from the CSV file.

## Type conversion

The columns can be automatically parsed to a specific type by suffixing the column name in the CSV header with `::type`.

Currently, the following types are supported:

- `bool`
- `datetime`
- `double`
- `int`
- `string` (default)

## Use Cases

### How to convert a CSV file

1. **Drop your file** in the following folder:

   `C:\Skyline DataMiner\Documents\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadCsvFile`
  
   > [!NOTE]
   >
   > - If the specified path does not exist, GQI will automatically creates it, allowing users to add the CSV file later.
   > - File Reader will also detect the delimiter when none is specified, and supports adjusting the capitalization of table headers based on user-provided arguments, making it both flexible and user-friendly.

1. **Run the corresponding script**, and File Reader will process your data instantly.  
1. **Visualize, analyze, and integrate** the processed data into your system.  

### Example of a CSV file

```CSV
Timestamp::datetime,Test name,Test cases::int,Duration::double,Success::boolean
06/12/2023 01:00,Cisco CMTS,36,1081.788,false
06/12/2023 01:21,Huawei 5600 5800,4,196.621,true
06/12/2023 01:26,Cisco CBR8,41,1443.027,true
06/12/2023 01:50,Arris E6000,33,989.310,false
06/12/2023 02:08,Casa Systems,12,374.005,true
```
