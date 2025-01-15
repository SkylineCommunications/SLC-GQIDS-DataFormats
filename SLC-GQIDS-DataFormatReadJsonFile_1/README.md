# SLC-GQIDS-DataFormatReadJsonFile

GQI data source that can read in JSON files.

The file to be imported must be placed in the following path:
`C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadJsonFile`

If the specified path does not exist, the GQI automatically creates it, allowing users to add the json file later.

## Structure

The JSON file consists of three properties:

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
