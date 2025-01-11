namespace BlazorCssTransitions;

public partial class Spec
{
    public static Spec EaseOut(TimeSpan? duration = null, TimeSpan? delay = null)
        => new EaseOutSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
    public static Spec EaseOut(double? durationMs, double? delayMs = null)
        => new EaseOutSpecification(TimeSpan.FromMilliseconds(durationMs ?? 200), TimeSpan.FromMilliseconds(delayMs ?? 0));

    public readonly static Spec EaseOut100ms = EaseOut(TimeSpan.FromMilliseconds(100), delay: null);
    public readonly static Spec EaseOut200ms = EaseOut(TimeSpan.FromMilliseconds(200), delay: null);
    public readonly static Spec EaseOut500ms = EaseOut(TimeSpan.FromMilliseconds(500), delay: null);
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
