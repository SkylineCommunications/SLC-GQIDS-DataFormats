using System;
using Skyline.DataMiner.Analytics.GenericInterface;

/// <summary>
/// GQI operator that divides values from one column by values from another column.
/// Supports an optional modifier to scale the result.
/// </summary>
[GQIMetaData(Name = "Divide operator")]
public class MyCustomOperator : IGQIColumnOperator, IGQIRowOperator, IGQIInputArguments
{
	private readonly GQIColumnDropdownArgument _firstColumnArg = new GQIColumnDropdownArgument("First column") { IsRequired = true, Types = new GQIColumnType[] { GQIColumnType.Double } };
	private readonly GQIColumnDropdownArgument _secondColumnArg = new GQIColumnDropdownArgument("Second column") { IsRequired = true, Types = new GQIColumnType[] { GQIColumnType.Double } };
	private readonly GQIDoubleArgument _modifierArg = new GQIDoubleArgument("Modifier") { IsRequired = false };
	private readonly GQIStringArgument _nameArg = new GQIStringArgument("Column name") { IsRequired = true };

	private GQIColumn _firstColumn;
	private GQIColumn _secondColumn;
	private GQIDoubleColumn _newColumn;
	private double _modifier;

	public GQIArgument[] GetInputArguments()
	{
		return new GQIArgument[] { _firstColumnArg, _secondColumnArg, _nameArg, _modifierArg };
	}

	public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
	{
		_firstColumn = args.GetArgumentValue(_firstColumnArg);
		_secondColumn = args.GetArgumentValue(_secondColumnArg);
		_newColumn = new GQIDoubleColumn(args.GetArgumentValue(_nameArg));
		_modifier = args.GetArgumentValue(_modifierArg);
		if (_modifier == 0)
		{
			_modifier = 1;
		}

		return new OnArgumentsProcessedOutputArgs();
	}

	public void HandleColumns(GQIEditableHeader header)
	{
		header.AddColumns(_newColumn);
	}

	public void HandleRow(GQIEditableRow row)
	{
		var firstValue = row.GetValue<double>(_firstColumn);
		var secondValue = row.GetValue<double>(_secondColumn);

		// Handle division by zero
		double result;
		if (Math.Abs(secondValue) < double.Epsilon)
		{
			result = double.NaN;
		}
		else
		{
			result = _modifier * firstValue / secondValue;
		}

		row.SetValue(_newColumn, result, $"{result}");
	}
}