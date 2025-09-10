# Divide Operator

## About

This package contains a GQI operator that performs division operations between two numeric columns in your data.

## Key Features

### Mathematical Division Operations
  
The operator divides values from a first column (dividend) by values from a second column (divisor), creating a new result column.

### Optional Modifier Support

You can apply an optional multiplier to scale your division results. If no modifier is provided, the default value of 1 is used.

### Division by Zero Handling

The operator automatically detects division by zero scenarios and returns `NaN` (Not a Number) values to prevent calculation errors.

### Custom Column Naming

Define custom names for your calculated result columns to ensure clarity in your data analysis.

## Use Cases

### How to use the Divide Operator

1. **Add the operator** to any GQI query that contains numeric data columns.

2. **Configure the parameters**:
   - **First column** (required): Select the dividend column containing numeric values
   - **Second column** (required): Select the divisor column containing numeric values  
   - **Column name** (required): Provide a name for the new calculated column
   - **Modifier** (optional): Apply a multiplier to scale the results (default: 1)

3. **Execute the query** – The operator will calculate the division and add the result as a new column.

### Example Use Cases

#### Calculating Ratios
```
Revenue / Expenses = Profit Ratio
```

#### Performance Metrics
```
Successful Tests / Total Tests = Success Rate
```

#### Rate Calculations
```
Distance / Time = Speed
```

#### Efficiency Metrics
```
Output / Input * 100 = Efficiency Percentage (using modifier = 100)
```

### Example Configuration

- **First column**: "Total Sales"
- **Second column**: "Number of Orders" 
- **Column name**: "Average Order Value"
- **Modifier**: 1 (or leave empty for default)

Result: Creates a new column "Average Order Value" with values calculated as (Total Sales ÷ Number of Orders).

## Technical Notes

- Only supports columns with **double** (numeric) data types
- Division by zero returns `NaN` values instead of causing errors
- If modifier is set to 0, it defaults to 1 to prevent unintended zero multiplication
- Result values are displayed with their calculated numeric representation

## Error Handling

The operator includes robust error handling:
- **Invalid column types**: Only numeric (double) columns can be selected
- **Division by zero**: Results in `NaN` values rather than exceptions
- **Missing values**: Handled gracefully according to GQI standards