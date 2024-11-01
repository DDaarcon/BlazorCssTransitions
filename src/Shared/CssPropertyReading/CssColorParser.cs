using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.Shared.CssPropertyReading;

internal static class CssColorParser
{
	public static Color? AttemptParse(ReadOnlySpan<char> value)
	{
		if (value.StartsWith("rgba"))
			return ParseRgba(value);
		if (value.StartsWith("rgb"))
			return ParseRgb(value);


		return Color.Beige;
	}

	private static Color? ParseRgba(ReadOnlySpan<char> value)
	{
		value = value[5..];

		var numbers = FindAllNumbers(value);

		if (numbers.Count != 4)
			return null;

		return Color.FromArgb(numbers[3], numbers[0], numbers[1], numbers[2]);
	}

	private static Color? ParseRgb(ReadOnlySpan<char> value)
	{
		value = value[4..];

		var numbers = FindAllNumbers(value);

		if (numbers.Count != 3)
			return null;

		return Color.FromArgb(numbers[0], numbers[1], numbers[2]);
	}

	private static List<int> FindAllNumbers(ReadOnlySpan<char> value)
	{
		var numbers = new List<int>();
		var delimOrEnd = value.IndexOfAny(',', ')');
		while (delimOrEnd != -1)
		{
			var possibleNumber = value[0..delimOrEnd];

			if (int.TryParse(possibleNumber, out var number))
			{
				numbers.Add(number);
			}
			else
				break;

			value = value[(delimOrEnd + 1)..];
			delimOrEnd = value.IndexOfAny(',', ')');
		}

		return numbers;
	}
}
