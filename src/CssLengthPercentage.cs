using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.Exceptions;
using BlazorCssTransitions.ValueValidators.PropertySyntaxVaidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public readonly struct CssLengthPercentage
{
	public static implicit operator CssLengthPercentage(string value) => new(value);
	public static implicit operator CssLengthPercentage(CssLength value) => new(value);
	public static implicit operator CssLengthPercentage(CssPercentage value) => new(value);
	public static implicit operator string(CssLengthPercentage value) => value.ToString();

	public override string ToString()
		=> Assertions.AssertNotNullAndGet(_value, $"{typeof(CssLengthPercentage).Name} does not have a value");

	private readonly string? _value;
	public bool IsAssigned => _value is not null;

	public CssLengthPercentage(string value)
	{
		if (!CssLengthPercentagePropertySyntaxValidator.Validate(value))
			throw ValueParsingException.ThrowFor<CssLengthPercentage>(value);

		_value = value;
	}

}
