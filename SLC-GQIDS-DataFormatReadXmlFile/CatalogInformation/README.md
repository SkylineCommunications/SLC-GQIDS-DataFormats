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