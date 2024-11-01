using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.Shared;

internal static class ColorExtensions
{
	public static string ToHex(this Color color)
		=> $"#{color.R:X2}{color.G:X2}{color.B:X2}";
}
