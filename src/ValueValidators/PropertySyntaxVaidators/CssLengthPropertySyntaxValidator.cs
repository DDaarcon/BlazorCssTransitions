using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.ValueValidators.PropertySyntaxVaidators;

// TODO tests
internal static class CssLengthPropertySyntaxValidator
{
    public static bool Validate(ReadOnlySpan<char> value)
    {
        var (foundUnit, unitLength) = ValidateUnitPresenceAndGetUnitLength(value);
        
        if (foundUnit)
        {
            return ValidateIfIsNumber(value.Slice(0, value.Length - 2));
        }

        // TODO should we pass any number?
        return ValidateIfIsNumber(value);
    }

    private readonly static string[] _units = [
        "px",
        "cm",
        "mm",
        "Q",
        "in",
        "pc",
        "pt",
		"cap",
        "ch",
        "em",
        "ex",
        "ic",
        "lh",
        "rcap",
        "rch",
        "rem",
        "rex",
        "ric",
        "rlh",
        "vh",
        "vw",
        "vmax",
        "vmin",
        "vb",
        "vi",
        "cqw",
        "cqh",
        "cqi",
        "cqb",
        "cqmin",
        "cqmax"
		];

    private static (bool isFound, int foundUnitLength) ValidateUnitPresenceAndGetUnitLength(ReadOnlySpan<char> value)
    {
        foreach (var unit in _units)
        {
            if (value.EndsWith(unit))
                return (true, unit.Length);
        }

        return (false, 0);
    }

	private static bool ValidateIfIsNumber(ReadOnlySpan<char> value)
    {
        return float.TryParse(value, ValidatorsSharedItems.DotDelimeterCulture, out _);
    }
}
