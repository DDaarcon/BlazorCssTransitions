using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.Exceptions;
using BlazorCssTransitions.ValueValidators.PropertySyntaxVaidators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public readonly struct CssPercentage : IComparable<CssPercentage>
{
	public static implicit operator CssPercentage(float numericValue) => new($"{numericValue}%");
	public static implicit operator CssPercentage(string value) => new(value);
	public static implicit operator string(CssPercentage value) => value.ToString();
	public override string ToString()
		=> Assertions.AssertNotNullAndGet(_value, $"{typeof(CssPercentage).Name} does not have a value");

	private readonly string? _value;
	private readonly float _asNumeric;
	private readonly bool _isWithSign;

	public float AsNumeric => _asNumeric;
	public bool IsAssigned => _value is not null;

	public CssPercentage(string value)
	{
		var validationResult = CssPercentagePropertySyntaxValidator.Validate(value);
		if (!validationResult.IsValid)
			throw ValueParsingException.ThrowFor<CssPercentage>(value);

		_value = value;
		_isWithSign = validationResult.ContainsSign;

		_asNumeric = _isWithSign
			? float.Parse(ToString())
			: 0; /*only 0 is allowed without sign*/
    }

	public static CssPercentage Unassigned() => new();

    public int CompareTo(CssPercentage other)
		=> Comparer<float>.Default.Compare(AsNumeric, other.AsNumeric);
}
