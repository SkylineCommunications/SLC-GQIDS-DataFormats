# DataFormats

## Unlock the Power of Seamless Data Transformation
Easily convert and visualize data from **CSV, JSON, and XML** files with **DataFormats**. Whether you're an analyst, developer, or business user, this package helps you streamline your workflow and extract insights from raw data with minimal effort.

### Why Choose DataFormats?
✅ **Effortless Data Processing** – Simply place your file in the right folder, and let DataFormats handle the rest. No complex setup required!  
✅ **Smart Delimiter Detection** – Automatically identifies delimiters in CSV files, ensuring smooth data imports.  
✅ **Flexible Type Conversion** – Supports **bool, datetime, double, int,** and **string** formats for seamless data transformation.  
✅ **Automated Directory Creation** – Missing the required folder? No worries—DataFormats creates it for you.  
✅ **Multi-Format Compatibility** – Works with CSV, JSON, and XML, making it the perfect all-in-one data processing solution.

### How It Works
1️⃣ **Drop your file** in the designated directory.  
2️⃣ **Run the corresponding script**, and DataFormats will process your data instantly.  
3️⃣ **Visualize, analyze, and integrate** the processed data into your system.  


## Supported Formats

### CSV Processing
📂 **File Path:**  
`C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadCsvFile`

🔹 **Smart Delimiter Detection** – Identifies `,` `;` `\t` `|` automatically.  

🔹 **Header Capitalization Control** – Choose how headers should be formatted.  

🔹 **Type Conversion** – Define column types using `::type` suffixes.

🔹 **Supports All Common Data Types** (`bool, datetime, double, int, string (default)`).


🔹 **Example CSV Format:**
```csv
Timestamp::datetime,Test Name,Test Cases::int,Duration::double,Success::boolean
06/12/2023 01:00,Cisco CMTS,36,1081.788,false
06/12/2023 01:21,Huawei 5600 5800,4,196.621,true
```

### JSON Processing
📂 **File Path:**  
`C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadJsonFile`

🔹 **Column-based Structure** – Define each column with `Name` and `Type`.

🔹 **Supports Structured JSON** for easy integration.

🔹 **Supports All Common Data Types** (`bool, datetime, double, int, string`).


🔹 **Example JSON Format:**
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
`C:\Skyline DataMiner\Documents\DataMiner Catalog\DevOps\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadXmlFile`

🔹 **Structured Data Extraction** – Define columns and rows for seamless parsing.  

🔹 **Supports All Common Data Types** (`bool, datetime, double, int, string`).


🔹 **Example XML Format:**
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

## Get Started Today!
Experience efficient and seamless data transformation with **DataFormats**. Designed for handling CSV, JSON, and XML files, this package optimizes your workflow by automating data processing and integration.

For **technical details**, advanced configuration, and troubleshooting, refer to the **full documentation** for each script:

- 📄 [CSV File Readme](https://github.com/SkylineCommunications/SLC-GQIDS-DataFormats/blob/master/SLC-GQIDS-DataFormatReadCsvFile_1/README.md)
- 📄 [JSON File Readme](https://github.com/SkylineCommunications/SLC-GQIDS-DataFormats/blob/master/SLC-GQIDS-DataFormatReadJsonFile_1/README.md)
- 📄 [XML File Readme](https://github.com/SkylineCommunications/SLC-GQIDS-DataFormats/blob/master/SLC-GQIDS-DataFormatReadXmlFile_1/README.md)

small change
