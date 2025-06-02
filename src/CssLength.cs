using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.Exceptions;
using BlazorCssTransitions.ValueValidators.PropertySyntaxVaidators;

namespace BlazorCssTransitions;

public readonly struct CssLength
{
	public static implicit operator CssLength(string value) => new(value);
	public override string ToString()
		=> !_isUnassigned
		? Assertions.AssertNotNullAndGet(_value, $"{typeof(CssLength).Name} does not have a value")
		: null!;

	private readonly string? _value;
	private bool _isUnassigned { get; init; }
	public bool IsAssigned => !_isUnassigned;

	public CssLength(string value)
	{
		if (value is null)
			throw ValueParsingException.NewFor<CssLength>(value);

        if (!CssLengthPropertySyntaxValidator.Validate(value))
			throw ValueParsingException.NewFor<CssLength>(value);

		_value = value;
	}

	public static CssLength Unassigned() => new()
	{
		_isUnassigned = true
	};
}
