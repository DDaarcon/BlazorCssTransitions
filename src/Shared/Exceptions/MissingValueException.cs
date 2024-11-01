using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.Shared.Exceptions;
public class MissingValueException : Exception
{
	public MissingValueException(string? message) : base(message)
	{ }
}
