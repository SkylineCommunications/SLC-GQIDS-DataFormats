# SLC-GQIDS-DataFormats

this package contains three different scripts that allow users to visualize data from CSV, JSON and XML files.

## Scripts Overview
* SLC-GQIDS-DataFormatReadCsvFile   
  - GQI name: `CSV File`
* SLC-GQIDS-DataFormatReadJsonFile
  - GQI name: `JSON File`
* SLC-GQIDS-DataFormatReadXmlFile
  - GQI name: `XML File`

### SLC-GQIDS-DataFormatReadCsvFile
---

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

### SLC-GQIDS-DataFormatReadJsonFile
---

GQI data source that can read in JSON files.

The file to be imported must be placed in the following path:
`C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad-Hoc Data Sources\SLC-GQIDS-DataFormatReadJsonFile`

If the specified path does not exist, the GQI automatically creates it, allowing users to add the json file later.

## Structure

The JSON file consists of two properties:

* Columns
* Rows


### Columns

An array containing the columns of the data set. A column consists of 2 properties:

* Name
* Type

The name is a string and contains the display name of the column.
The type is a string that identifies the content type of that column.

Currently the following types are available:

* boolean
* datetime
* double
* int
* string

### Rows

An array containing the rows of the data set. Every row contains of an array of cells.

Every cell consists of 2 properties.

* Value
* DisplayValue

The value can be a string or number.
The display value is a string representation of the value.

## Example

```json
{
    "Columns": [
        {
            "Name": "Order ID",
            "Type": "string"
        },
        {
            "Name": "Created",
            "Type": "datetime"
        },
        {
            "Name": "Customer",
            "Type": "string"
        },
        {
            "Name": "Profit",
            "Type": "Double"
        }
    ],
    "Rows": [
        {
            "Cells": [
                {
                    "Value": 149384
                },
                {
                    "DisplayValue": "Nov 3, 2023",
                    "Value": 1699009200000
                },
                {
                    "Value": "Sebastiaan Dumoulein"
                },
                {
                    "DisplayValue": "$604.50",
                    "Value": 604.50
                }
            ]
        },
        {
            "Cells": [
                {
                    "Value": 153322
                },
                {
                    "DisplayValue": "Oct 28, 2023",
                    "Value": 1698487200000
                },
                {
                    "Value": "Abbas Melendez"
                },
                {
                    "DisplayValue": "$307.70",
                    "Value": 307.70
                }
            ]
        }
    ]
}
```

### SLC-GQIDS-DataFormatReadXmlFile
---

# SLC-GQIDS-DataFormatReadXmlFile

GQI data source that can read in XML files.

The file to be imported must be placed in the following path:
`C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad-Hoc Data Sources\SLC-GQIDS-DataFormatReadXmlFile`

If the specified path does not exist, the GQI automatically creates it, allowing users to add the XML file later.

## Structure

The XML file consists of two properties:

* Columns
* Rows


### Columns

An array containing the columns of the data set. A column consists of 2 properties:

* Name
* Type

The name is a string and contains the display name of the column.
The type is a string that identifies the content type of that column.

Currently the following types are available:

* boolean
* datetime
* double
* int
* string

### Rows

An array containing the rows of the data set. Every row contains of an array of cells.

Every cell consists of 2 properties.

* Value
* DisplayValue

The value can be a string or number.
The display value is a string representation of the value.

## Example

```xml
<Data>
    <Columns>
        <Column>
            <Name>Order ID</Name>
            <Type>string</Type>
        </Column>
        <Column>
            <Name>Created</Name>
            <Type>datetime</Type>
        </Column>
        <Column>
            <Name>Customer</Name>
            <Type>string</Type>
        </Column>
        <Column>
            <Name>Profit</Name>
            <Type>Double</Type>
        </Column>
    </Columns>
    <Rows>
        <Row>
            <Cells>
                <Cell>
                    <Value>149384</Value>
                </Cell>
                <Cell>
                    <DisplayValue>Nov 3, 2023</DisplayValue>
                    <Value>1699009200000</Value>
                </Cell>
                <Cell>
                    <Value>Sebastiaan Dumoulein</Value>
                </Cell>
                <Cell>
                    <DisplayValue>$604.50</DisplayValue>
                    <Value>604.50</Value>
                </Cell>
            </Cells>
        </Row>
        <Row>
            <Cells>
                <Cell>
                    <Value>153322</Value>
                </Cell>
                <Cell>
                    <DisplayValue>Oct 28, 2023</DisplayValue>
                    <Value>1698487200000</Value>
                </Cell>
                <Cell>
                    <Value>Abbas Melendez</Value>
                </Cell>
                <Cell>
                    <DisplayValue>$307.70</DisplayValue>
                    <Value>307.70</Value>
                </Cell>
            </Cells>
        </Row>
    </Rows>
</Data>
```

## License

This project is licensed under the MIT License. See the `LICENSE.txt` file for details.
