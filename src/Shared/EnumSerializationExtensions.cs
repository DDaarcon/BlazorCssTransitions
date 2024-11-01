using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.Shared;
internal static class EnumSerializationExtensions
{
	public static string ToCss(this PropertySyntax syntax)
		=> syntax switch
		{
			PropertySyntax.Length => "<length>",
			PropertySyntax.Number => "<number>",
			PropertySyntax.Percentage => "<percentage>",
			PropertySyntax.LengthPercentage => "<length-percentage>",
			PropertySyntax.Color => "<color>",
			PropertySyntax.Integer => "<integer>",
			PropertySyntax.Angle => "<angle>",
			PropertySyntax.Time => "<time>",
			_ => throw new NotImplementedException($"Incorrect value for PropertySyntax ({syntax})"),
		};

	public static string ToCss(this AnimationDirection direction)
		=> direction switch
		{
			AnimationDirection.Normal => "normal",
			AnimationDirection.Reverse => "reverse",
			AnimationDirection.Alternate => "alternate",
			AnimationDirection.AlternateReverse => "alternate-reverse",
			_ => throw new NotImplementedException($"Incorrect value for AnimationDirection ({direction})"),
		};

	public static string ToCss(this AnimationFillMode fillMode)
		=> fillMode switch
		{
			AnimationFillMode.None => "none",
			AnimationFillMode.Forwards => "forwards",
			AnimationFillMode.Both => "both",
			AnimationFillMode.Backwards => "backwards",
			_ => throw new NotImplementedException($"Incorrect value for AnimationFillMode ({fillMode})"),
		};
}
