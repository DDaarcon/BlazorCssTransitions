using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.Exceptions;
using BlazorCssTransitions.ValueValidators.PropertySyntaxVaidators;

namespace BlazorCssTransitions;

public readonly struct CssLengthPercentage
{
	public static implicit operator CssLengthPercentage(string value) => new(value);
	public static implicit operator CssLengthPercentage(CssLength value) => new(value.ToString());
	public static implicit operator CssLengthPercentage(CssPercentage value) => new(value.ToString());

	public override string ToString()
		=> !_isUnassigned
		? Assertions.AssertNotNullAndGet(_value, $"{typeof(CssLengthPercentage).Name} does not have a value")
		: null!;

	private readonly string? _value;
    private bool _isUnassigned { get; init; }
    public bool IsAssigned => !_isUnassigned;

    public CssLengthPercentage(string value)
    {
        if (value is null)
            throw ValueParsingException.NewFor<CssLengthPercentage>(value);

        if (!CssLengthPercentagePropertySyntaxValidator.Validate(value))
			throw ValueParsingException.NewFor<CssLengthPercentage>(value);

		_value = value;
	}

	public static CssLengthPercentage Unassigned() => new()
    {
        _isUnassigned = true
    };
}
