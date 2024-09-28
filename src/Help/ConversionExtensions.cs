using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.Help;

internal static class ConversionExtensions
{
    public static string ToCssDuration(this TimeSpan duration)
    {
        if (duration.TotalSeconds >= 1)
            return $"{Math.Floor(duration.TotalSeconds)}.{duration.Milliseconds / 10}s";

        return $"{duration.Milliseconds}ms";
    }

    public static string ToCss(this float value)
        => value.ToString("0.##", CultureInfo.InvariantCulture);

    public static string ToCss(this double value)
        => value.ToString("0.##", CultureInfo.InvariantCulture);
}
