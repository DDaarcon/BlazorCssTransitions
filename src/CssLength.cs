﻿using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.Exceptions;
using BlazorCssTransitions.ValueValidators.PropertySyntaxVaidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public readonly struct CssLength
{
	public static implicit operator CssLength(string value) => new(value);
	public override string ToString()
		=> Assertions.AssertNotNullAndGet(_value, $"{typeof(CssLength).Name} does not have a value");

	private readonly string? _value;
	public bool IsAssigned => _value is not null;

	public CssLength(string value)
	{
		if (!CssLengthPropertySyntaxValidator.Validate(value))
			throw ValueParsingException.NewFor<CssLength>(value);

		_value = value;
	}

	public static CssLength Unassigned() => new();
}
