# DataFormats

This package provides a seamless way to visualize and transform data from **CSV, JSON, and XML** files, making data integration easier and more efficient.

## Key Features
- **Multi-format Support**: Handles CSV, JSON, and XML data effortlessly.
- **Smart Detection**: Automatically detects delimiters and adjusts header capitalization.
- **Flexible Type Conversion**: Supports automatic parsing of data types.
- **Automated Directory Creation**: Ensures files are placed in the right location.

## Scripts Overview
* **SLC-GQIDS-DataFormatReadCsvFile**  
  - Library name: `CSV File`
* **SLC-GQIDS-DataFormatReadJsonFile**
  - Library name: `JSON File`
* **SLC-GQIDS-DataFormatReadXmlFile**
  - Library name: `XML File`

### CSV File Processing
A custom Ad hoc data source to import values from CSV files.

**File Path:**  
`C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad-Hoc Data Sources\SLC-GQIDS-DataFormatReadCsvFile`

If the specified path does not exist, the Ad hoc data source automatically creates it, allowing users to add the file later. The solution detects the delimiter when none is specified and supports header capitalization adjustments based on user-provided arguments.

### Smart Features
#### Delimiter Detection
If no delimiter is provided, the Ad hoc data source detects the most common delimiter in the header from:
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

### Example CSV Format
```csv
Timestamp::datetime,Test Name,Test Cases::int,Duration::double,Success::boolean
06/12/2023 01:00,Cisco CMTS,36,1081.788,false
06/12/2023 01:21,Huawei 5600 5800,4,196.621,true
06/12/2023 01:26,Cisco CBR8,41,1443.027,true
06/12/2023 01:50,Arris E6000,33,989.310,false
06/12/2023 02:08,Casa Systems,12,374.005,true
```

### JSON File Processing
Processes JSON files, mapping structured data into an accessible format with defined columns and rows.

**File Path:**  
`C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad-Hoc Data Sources\SLC-GQIDS-DataFormatReadJsonFile`

#### Column Types
Each column in a JSON file consists of:
- **Name**: Display name of the column.
- **Type**: Data type, supporting:
  - `bool`
  - `datetime`
  - `double`
  - `int`
  - `string`

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

#### Column Types
Each column in an XML file consists of:
- **Name**: Display name of the column.
- **Type**: Data type, supporting:
  - `bool`
  - `datetime`
  - `double`
  - `int`
  - `string`

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
