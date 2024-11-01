using BlazorCssTransitions.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.Shared;

internal static class Assertions
{
    internal static TProperty AssertNotNullAndGet<TProperty>(ref TProperty? nullableValue, string missingExceptionMessage)
        where TProperty : struct
    {
        if (nullableValue.HasValue)
            return nullableValue.Value;

        throw new MissingValueException(missingExceptionMessage);
    }

    internal static TProperty AssertNotNullAndGet<TProperty>(ref TProperty? value, string missingExceptionMessage)
        where TProperty : class
    {
        if (value is not null)
            return value;

        throw new MissingValueException(missingExceptionMessage);
    }

    internal static TProperty AssertNotNullAndGet<TProperty>(TProperty? value, string missingExceptionMessage)
        where TProperty : class
    {
        if (value is not null)
            return value;

        throw new MissingValueException(missingExceptionMessage);
    }
}
