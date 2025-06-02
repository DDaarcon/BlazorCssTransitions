namespace BlazorCssTransitions.Shared.Exceptions;

public class ValueParsingException : Exception
{
	public ValueParsingException(string? invalidValue, string typeName)
		: base($"Value {invalidValue} can not be parsed into {typeName}")
	{

	}

	public static ValueParsingException NewFor<TExpectedType>(string? invalidValue)
		=> new(invalidValue, typeof(TExpectedType).Name);
}
