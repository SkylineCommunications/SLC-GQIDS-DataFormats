# File Reader

## About

This package allows you to easily convert and visualize data from **CSV, JSON, and XML files**.

Whether you are an analyst, developer, or business user, this package will help you streamline your workflow and extract insights from raw data with minimal effort.

> [!TIP]
> Easily see how to implement a Dashboard/Low Code app with these scripts with our [Generic File Reader GQI Tutorial](https://catalog.dataminer.services/details/f7ecd365-7bf9-406d-978f-eaca9e3aa9c2).

## Key Features

### General features

- **Effortless data processing** – Simply place your file in the right folder, and let File Reader handle the rest. No complex setup required.
- **Smart delimiter detection** – Automatically identifies delimiters in CSV files, ensuring smooth data imports.  
- **Flexible type conversion** – Supports **bool, datetime, double, int, and string** formats for seamless data transformation.  
- **Automated directory creation** – Missing the required folder? No worries. File Reader will create it for you.  
- **Multi-format compatibility** – Works with CSV, JSON, and XML, making it the perfect all-in-one data processing solution.

### CSV-related features

- **Smart delimiter detection** – Identifies `,` `;` `\t` `|` automatically.  
- **Header capitalization control** – Choose how headers should be formatted.  
- **Type conversion** – Define column types using `::type` suffixes.
- **Supports all common data types** - `bool`, `datetime`, `double`, `int`, `string` (default)

### JSON-related features

- **Column-based structure** – Define each column with `Name` and `Type`.
- **Supports structured JSON** - For easy integration.
- **Supports all common data types** - `bool`, `datetime`, `double`, `int`, `string`

### XML-related features

- **Structured data extraction** – Define columns and rows for seamless parsing.  
- **Supports all common data types** - `bool`, `datetime`, `double`, `int`, `string`

## Use Cases

### How to convert a CSV, JSON, or XML file

1. **Drop your file** in the designated folder:

   | File | Folder |
   |------|--------|
   | CSV file  | `C:\Skyline DataMiner\Documents\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadCsvFile`  |
   | JSON file | `C:\Skyline DataMiner\Documents\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadJsonFile` |
   | XML file  | `C:\Skyline DataMiner\Documents\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadXmlFile`  |
  
1. **Run the corresponding script**, and File Reader will process your data instantly.  
1. **Visualize, analyze, and integrate** the processed data into your system.  

### Example CSV format

```csv
Timestamp::datetime,Test Name,Test Cases::int,Duration::double,Success::boolean
06/12/2023 01:00,Cisco CMTS,36,1081.788,false
06/12/2023 01:21,Huawei 5600 5800,4,196.621,true
```

### Example JSON format

```json
{
    "Columns": [
        { "Name": "Order ID", "Type": "string" },
        { "Name": "Created", "Type": "datetime" }
    ],
    "Rows": [
        {
            "Cells": [
                { "Value": 149384 },
                { "DisplayValue": "Nov 3, 2023", "Value": 1699009200000 }
            ]
        }
    ]
}
```

### Example XML format

```xml
<Data>
    <Columns>
        <Column>
            <Name>Order ID</Name>
            <Type>string</Type>
        </Column>
    </Columns>
    <Rows>
        <Row>
            <Cells>
                <Cell>
                    <Value>149384</Value>
                </Cell>
            </Cells>
        </Row>
    </Rows>
</Data>
```

## Technical Reference

For **technical details**, advanced configuration, and troubleshooting, see the **full documentation** for each script:

- [CSV File Reader](https://catalog.dataminer.services/details/2cebdc7f-4e9c-42f4-9cb8-65938062abc0)
- [JSON File Reader](https://catalog.dataminer.services/details/b0c3e2ab-6827-43b4-9b25-1299cd1e97ae)
- [XML File Reader](https://catalog.dataminer.services/details/a2e5d318-642a-4c05-b75a-177d0d5eb18b)
