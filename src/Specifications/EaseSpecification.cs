using BlazorCssTransitions.Shared;

namespace BlazorCssTransitions.Specifications;

public partial class Spec
{
    public static Spec Ease(TimeSpan? duration = null, TimeSpan? delay = null)
        => new EaseSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
    public static Spec Ease(double? durationMs, double? delayMs = null)
        => new EaseSpecification(TimeSpan.FromMilliseconds(durationMs ?? 200), TimeSpan.FromMilliseconds(delayMs ?? 0));
}

internal class EaseSpecification : Spec
{
    public EaseSpecification(TimeSpan duration, TimeSpan delay)
    {
        _duration = duration;
        _delay = delay;
        _timingFunction = "ease";
    }
}
