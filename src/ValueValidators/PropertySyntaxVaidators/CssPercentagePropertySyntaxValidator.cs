using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.ValueValidators.PropertySyntaxVaidators;

internal static class CssPercentagePropertySyntaxValidator
{
	public static Result Validate(ReadOnlySpan<char> value)
	{
		var isPercentageSignPresent = ValidatePercetageSignPresence(value);

		if (!isPercentageSignPresent)
		{
			return new Result(value == "0", false);
		}

		return new Result(ValidateIfIsNumber(value[..^1]), true);
	}

	private static bool ValidatePercetageSignPresence(ReadOnlySpan<char> value)
	{
		return value.EndsWith("%");
	}

	private static bool ValidateIfIsNumber(ReadOnlySpan<char> value)
	{
		return float.TryParse(value, ValidatorsSharedItems.DotDelimeterCulture, out _);
	}

	internal record struct Result(
		bool IsValid,
		bool ContainsSign);
}
