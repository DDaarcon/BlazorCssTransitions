using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.Shared.Exceptions;

public class ValueParsingException : Exception
{
	public ValueParsingException(string invalidValue, string typeName)
		: base($"Value {invalidValue} can not be parsed into {typeName}")
	{

	}

	public static ValueParsingException ThrowFor<TExpectedType>(string invalidValue)
		=> new(invalidValue, typeof(TExpectedType).Name);
}
