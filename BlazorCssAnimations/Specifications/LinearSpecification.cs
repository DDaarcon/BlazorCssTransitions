using BlazorCssAnimations.AnimatedVisibilityTransitions;
using BlazorCssAnimations.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssAnimations.Specifications;

public partial class Specification
{
    public static Specification Linear(TimeSpan? duration = null, TimeSpan? delay = null)
        => new LinearSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
}

internal class LinearSpecification : Specification
{
    public LinearSpecification(TimeSpan duration, TimeSpan delay)
    {
        _duration = duration;
        _delay = delay;
        _timingFunction = "linear";
    }
}
