# XML File Reader

## About

This package contains a GQI data source that can read JSON files.

> [!TIP]
> Easily see how to implement a Dashboard/Low Code app with these scripts with our [Generic File Reader GQI Tutorial](https://catalog.dataminer.services/details/f7ecd365-7bf9-406d-978f-eaca9e3aa9c2).

## Use Cases

### How to convert an XML file

1. **Drop your file** in the following folder:

   `C:\Skyline DataMiner\Documents\Ad Hoc Data Sources\SLC-GQIDS-DataFormatReadXmlFile`
  
   > [!NOTE]
   > If the specified path does not exist, GQI will automatically creates it, allowing users to add the XML file later.

1. **Run the corresponding script**, and File Reader will process your data instantly.  
1. **Visualize, analyze, and integrate** the processed data into your system.  

### XML file structure

The XML file consists of the following properties:

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

#### Example of an XML file

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