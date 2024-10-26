namespace BlazorCssTransitions.Specifications;

public partial class Spec
{
    public static Spec EaseOut(TimeSpan? duration = null, TimeSpan? delay = null)
        => new EaseOutSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
    public static Spec EaseOut(double? durationMs, double? delayMs = null)
        => new EaseOutSpecification(TimeSpan.FromMilliseconds(durationMs ?? 200), TimeSpan.FromMilliseconds(delayMs ?? 0));
}

internal class EaseOutSpecification : Spec
{
    public EaseOutSpecification(TimeSpan duration, TimeSpan delay)
    {
        _duration = duration;
        _delay = delay;
        _timingFunction = "ease-out";
    }
}
