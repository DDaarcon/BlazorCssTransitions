using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.Exceptions;
using BlazorCssTransitions.ValueValidators.PropertySyntaxVaidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public readonly struct CssPercentage
{
	public static implicit operator CssPercentage(float numericValue) => new($"{numericValue}%");
	public static implicit operator CssPercentage(string value) => new(value);
	public static implicit operator string(CssPercentage value) => value.ToString();
	public override string ToString()
		=> Assertions.AssertNotNullAndGet(_value, $"{typeof(CssPercentage).Name} does not have a value");

	private readonly string? _value;
	private readonly bool _isWithSign;

	public bool IsAssigned => _value is not null;

	public CssPercentage(string value)
	{
		var validationResult = CssPercentagePropertySyntaxValidator.Validate(value);
		if (!validationResult.IsValid)
			throw ValueParsingException.ThrowFor<CssPercentage>(value);

		_value = value;
		_isWithSign = validationResult.ContainsSign;
	}

	public static CssPercentage Unassigned() => new();

	internal float ToNumeric()
		=> _isWithSign
			? float.Parse(ToString())
			: 0; // only 0 is allowed without sign
}
