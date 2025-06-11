# File Reader

## About

This package allows you to easily convert and visualize data from **CSV, JSON, and XML files**.

Whether you are an analyst, developer, or business user, this package will help you streamline your workflow and extract insights from raw data with minimal effort.

## Key Features

- **Effortless Data Processing** – Simply place your file in the right folder, and let DataFormats handle the rest. No complex setup required!  
- **Smart Delimiter Detection** – Automatically identifies delimiters in CSV files, ensuring smooth data imports.  
- **Flexible Type Conversion** – Supports **bool, datetime, double, int,** and **string** formats for seamless data transformation.  
- **Automated Directory Creation** – Missing the required folder? No worries—DataFormats creates it for you.  
- **Multi-Format Compatibility** – Works with CSV, JSON, and XML, making it the perfect all-in-one data processing solution.

## Use Cases

### Converting a CSV, JSON, or XML file

1. **Drop your file** in the designated directory.  
1. **Run the corresponding script**, and DataFormats will process your data instantly.  
1. **Visualize, analyze, and integrate** the processed data into your system.  

### About processing CSV files

📂 **File Path:**  
`C:\Skyline DataMiner\Documents\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadCsvFile`

- **Smart Delimiter Detection** – Identifies `,` `;` `\t` `|` automatically.  
- **Header Capitalization Control** – Choose how headers should be formatted.  
- **Type Conversion** – Define column types using `::type` suffixes.
- **Supports All Common Data Types** (`bool, datetime, double, int, string (default)`).
- **Example CSV Format:**

```csv
Timestamp::datetime,Test Name,Test Cases::int,Duration::double,Success::boolean
06/12/2023 01:00,Cisco CMTS,36,1081.788,false
06/12/2023 01:21,Huawei 5600 5800,4,196.621,true
```

### JSON Processing

📂 **File Path:**  
`C:\Skyline DataMiner\Documents\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadJsonFile`

- **Column-based Structure** – Define each column with `Name` and `Type`.
- **Supports Structured JSON** for easy integration.
- **Supports All Common Data Types** (`bool, datetime, double, int, string`).
- **Example JSON Format:**

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

### XML Processing

📂 **File Path:**  
`C:\Skyline DataMiner\Documents\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadXmlFile`

- **Structured Data Extraction** – Define columns and rows for seamless parsing.  
- **Supports All Common Data Types** (`bool, datetime, double, int, string`).
- **Example XML Format:**

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

For **technical details**, advanced configuration, and troubleshooting, refer to the **full documentation** for each script:

- [CSV File Readme](https://github.com/SkylineCommunications/SLC-GQIDS-DataFormats/blob/master/SLC-GQIDS-DataFormatReadCsvFile_1/README.md)
- [JSON File Readme](https://github.com/SkylineCommunications/SLC-GQIDS-DataFormats/blob/master/SLC-GQIDS-DataFormatReadJsonFile_1/README.md)
- [XML File Readme](https://github.com/SkylineCommunications/SLC-GQIDS-DataFormats/blob/master/SLC-GQIDS-DataFormatReadXmlFile_1/README.md)
