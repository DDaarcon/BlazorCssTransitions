using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.Exceptions;
using BlazorCssTransitions.ValueValidators.PropertySyntaxVaidators;

namespace BlazorCssTransitions;

public readonly struct CssPercentage : IComparable<CssPercentage>
{
	public static implicit operator CssPercentage(float numericValue) => new($"{numericValue}%");
	public static implicit operator CssPercentage(string value) => new(value);
	public override string ToString()
		=> !_isUnassigned
		? Assertions.AssertNotNullAndGet(_value, $"{typeof(CssPercentage).Name} does not have a value")
		: null!;

	private readonly string? _value;
	private readonly float _asNumeric;
	private readonly bool _isWithSign;

	public float AsNumeric => _asNumeric;
    private bool _isUnassigned { get; init; }
    public bool IsAssigned => !_isUnassigned;

    public CssPercentage(string value)
    {
        if (value is null)
            throw ValueParsingException.NewFor<CssPercentage>(value);

        var validationResult = CssPercentagePropertySyntaxValidator.Validate(value);
		if (!validationResult.IsValid)
			throw ValueParsingException.NewFor<CssPercentage>(value);

		_value = value;
		_isWithSign = validationResult.ContainsSign;

		_asNumeric = _isWithSign
			? float.Parse(ToString())
			: 0; /*only 0 is allowed without sign*/
    }

	public static CssPercentage Unassigned() => new()
    {
        _isUnassigned = true
    };

    public int CompareTo(CssPercentage other)
		=> Comparer<float>.Default.Compare(AsNumeric, other.AsNumeric);
}
