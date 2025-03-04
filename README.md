# DataFormats

This package provides a seamless way to visualize and transform data from **CSV, JSON, and XML** files, making data integration easier and more efficient.

## Key Features
- **Multi-format Support**: Handles CSV, JSON, and XML data effortlessly.
- **Smart Detection**: Automatically detects delimiters and adjusts header capitalization.
- **Flexible Type Conversion**: Supports automatic parsing of data types.
- **Automated Directory Creation**: Ensures files are placed in the right location.

## Scripts Overview
* **SLC-GQIDS-DataFormatReadCsvFile**  
  - GQI name: `CSV File`
* **SLC-GQIDS-DataFormatReadJsonFile**
  - GQI name: `JSON File`
* **SLC-GQIDS-DataFormatReadXmlFile**
  - GQI name: `XML File`

### CSV File Processing
A custom GQI data source to import values from CSV files.

**File Path:**  
`C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad-Hoc Data Sources\SLC-GQIDS-DataFormatReadCsvFile`

If the specified path does not exist, the GQI automatically creates it, allowing users to add the file later. The solution detects the delimiter when none is specified and supports header capitalization adjustments based on user-provided arguments.

### Smart Features
#### Delimiter Detection
If no delimiter is provided, the GQI detects the most common delimiter in the header from:
- `,`
- `;`
- `\t`
- `|`

#### Header Capitalization
Allows users to specify the desired header capitalization method. If no value is provided, headers retain their original format.

#### Type Conversion
Columns can be automatically parsed using suffixes like `::type`, supporting:
- `bool`
- `datetime`
- `double`
- `int`
- `string` (default)

### JSON File Processing
Processes JSON files, mapping structured data into an accessible format with defined columns and rows.

**File Path:**  
`C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad-Hoc Data Sources\SLC-GQIDS-DataFormatReadJsonFile`

#### JSON Structure
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

### XML File Processing
Reads and processes structured XML data, ensuring consistency in column definitions and row data.

**File Path:**  
`C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad-Hoc Data Sources\SLC-GQIDS-DataFormatReadXmlFile`

#### XML Structure
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

## License
This project is licensed under the **MIT License**. See the `LICENSE.txt` file for details.
