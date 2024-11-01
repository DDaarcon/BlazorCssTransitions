using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.ValueValidators.PropertySyntaxVaidators;

internal static class CssLengthPercentagePropertySyntaxValidator
{
	public static bool Validate(ReadOnlySpan<char> value)
	{
		if (CssLengthPropertySyntaxValidator.Validate(value))
			return true;
		if (CssPercentagePropertySyntaxValidator.Validate(value).IsValid)
			return true;

		return false;
	}
}
