namespace BlazorCssTransitions.Specifications;

public partial class Spec
{
    public static Spec EaseInOut(TimeSpan? duration = null, TimeSpan? delay = null)
        => new EaseInOutSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
    public static Spec EaseInOut(double? durationMs, double? delayMs = null)
        => new EaseInOutSpecification(TimeSpan.FromMilliseconds(durationMs ?? 200), TimeSpan.FromMilliseconds(delayMs ?? 0));
}

internal class EaseInOutSpecification : Spec
{
    public EaseInOutSpecification(TimeSpan duration, TimeSpan delay)
    {
        _duration = duration;
        _delay = delay;
        _timingFunction = "ease-in-out";
    }
}
