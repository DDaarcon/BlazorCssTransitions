using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.Specifications;

internal static class SpecificationExtensions
{
    public static TimeSpan GetTotalDuration(this Spec specification)
        => specification.Duration + specification.Delay;

    public static TimeSpan GetLongestTotalDuration(this IEnumerable<Spec> specifications)
        => specifications.Select(x => x.Duration + x.Delay).Max();
}
