# GQI Data Formats

## About

This package offers a versatile set of GQI scripts designed to simplify the handling and manipulation of various data sources. Whether you're looking to load data for tabular display or perform custom operations, these scripts provide a flexible and efficient solution to get you started.

Whether you are an analyst, developer, or business user, this package will help you streamline your workflow and extract insights from raw data with minimal effort.

## Key Features

### File Reader Features

- **Effortless data processing** – Simply place your file in the right folder, and let File Reader handle the rest. No complex setup required.
- **Flexible type conversion** – Supports **bool, datetime, double, int, and string** formats for seamless data transformation.  
- **Multi-format compatibility** – Works with CSV, JSON, and XML, making it the perfect all-in-one data processing solution.

### Divide Operator Features

- **Mathematical operations** – Perform division operations between two numeric columns in your GQI data.
- **Modifier support** – Apply an optional multiplier to scale your division results.
- **Division by zero handling** – Automatically handles division by zero cases by returning NaN values.
- **Custom column naming** – Define custom names for your calculated result columns.

## Use Cases

### File Reader Use Case

1. **Drop your file** in the designated folder:

   | File | Folder |
   |------|--------|
   | CSV file  | `C:\Skyline DataMiner\Documents\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadCsvFile`  |
   | JSON file | `C:\Skyline DataMiner\Documents\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadJsonFile` |
   | XML file  | `C:\Skyline DataMiner\Documents\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadXmlFile`  |
  
1. **Run the corresponding script**, and File Reader will process your data instantly.  
1. **Visualize, analyze, and integrate** the processed data into your system.

### Divide Operator Use Case

1. **Add the Divide operator** to any GQI query that contains numeric data columns.
1. **Select your source columns** – Choose the dividend (first column) and divisor (second column).
1. **Configure the operation** – Set an optional modifier and provide a name for the result column.
1. **Execute the query** – The operator will calculate the division and add the result as a new column.

> [!TIP]
> Easily see how to implement a Dashboard/Low Code app with these scripts with our [Generic File Reader GQI Tutorial](https://catalog.dataminer.services/details/f7ecd365-7bf9-406d-978f-eaca9e3aa9c2).

## Technical Reference

For **technical details**, advanced configuration, and troubleshooting, see the **full documentation** for each script:

- **File Reader Adhoc Scripts**
    - [CSV File Reader](https://catalog.dataminer.services/details/2cebdc7f-4e9c-42f4-9cb8-65938062abc0)
    - [JSON File Reader](https://catalog.dataminer.services/details/b0c3e2ab-6827-43b4-9b25-1299cd1e97ae)
    - [XML File Reader](https://catalog.dataminer.services/details/a2e5d318-642a-4c05-b75a-177d0d5eb18b)

- **GQI Operators**
    - [Divide Operator](https://catalog.dataminer.services/details/7d2e8f3a-9b1c-4e5f-a6d7-1234567890ab)
