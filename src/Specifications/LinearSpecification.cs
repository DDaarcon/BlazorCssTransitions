using BlazorCssTransitions.AnimatedVisibilityTransitions;
using BlazorCssTransitions.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.Specifications;

public partial class Spec
{
    public static Spec Linear(TimeSpan? duration = null, TimeSpan? delay = null)
        => new LinearSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);

    public static Spec Linear(double? durationMs, double? delayMs = null)
        => new LinearSpecification(TimeSpan.FromMilliseconds(durationMs ?? 200), TimeSpan.FromMilliseconds(delayMs ?? 0));

	public readonly static Spec Linear100ms = Linear(TimeSpan.FromMilliseconds(100), delay: null);
	public readonly static Spec Linear200ms = Linear(TimeSpan.FromMilliseconds(200), delay: null);
	public readonly static Spec Linear500ms = Linear(TimeSpan.FromMilliseconds(500), delay: null);
}

internal class LinearSpecification : Spec
{
    public LinearSpecification(TimeSpan duration, TimeSpan delay)
    {
        _duration = duration;
        _delay = delay;
        _timingFunction = "linear";
    }
}
