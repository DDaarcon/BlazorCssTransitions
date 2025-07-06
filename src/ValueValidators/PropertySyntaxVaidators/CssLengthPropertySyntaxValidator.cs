namespace BlazorCssTransitions.ValueValidators.PropertySyntaxVaidators;

// TODO tests
internal static class CssLengthPropertySyntaxValidator
{
    public static bool Validate(ReadOnlySpan<char> value)
    {
        var (foundUnit, unitLength) = ValidateUnitPresenceAndGetUnitLength(value);
        
        if (foundUnit)
        {
            return ValidateIfIsNumber(value[..^unitLength]);
        }

        return ValidateIfIsNumber(value);
    }

    // have to be ordered by length, because of the way it is validated
    private readonly static string[] _units = [
        "cqmin",
        "cqmax",
        "rcap",
        "vmax",
        "vmin",
        "cqw",
        "cqh",
        "cqi",
        "cqb",
		"cap",
        "rch",
        "rem",
        "rex",
        "ric",
        "rlh",
        "px",
        "cm",
        "mm",
        "in",
        "pc",
        "pt",
        "ch",
        "em",
        "ex",
        "ic",
        "lh",
        "vh",
        "vw",
        "vb",
        "vi",
        "Q",
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
