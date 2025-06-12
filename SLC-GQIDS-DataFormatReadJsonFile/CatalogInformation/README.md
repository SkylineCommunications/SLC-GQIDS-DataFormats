# JSON File Reader

## About

This package contains a GQI data source that can read JSON files.

## Use Cases

### How to convert a JSON file

1. **Drop your file** in the following folder:

   `C:\Skyline DataMiner\Documents\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadJsonFile`
  
   > [!NOTE]
   > If the specified path does not exist, GQI will automatically create it, allowing users to add the JSON file later.

1. **Run the corresponding script**, and File Reader will process your data instantly.  
1. **Visualize, analyze, and integrate** the processed data into your system.  

### JSON file structure

The JSON file consists of the following properties:

- Columns
- Rows

#### Columns

An array containing the columns of the data set.

A column consists of the following properties:

- Name: A string containing the display name of the column.
- Type: A string containing the content type of the column.

Currently, the following content types are available:

- boolean
- datetime
- double
- int
- string

#### Rows

An array containing the rows of the data set.

Every row contains an array of cells, and every cell consists of the following properties:

- Value: A string or a number.
- DisplayValue: The string representation of the value.

#### Example of a JSON file

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
