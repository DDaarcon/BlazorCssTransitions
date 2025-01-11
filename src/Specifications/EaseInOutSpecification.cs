namespace BlazorCssTransitions;

public partial class Spec
{
    public static Spec EaseInOut(TimeSpan? duration = null, TimeSpan? delay = null)
        => new EaseInOutSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
    public static Spec EaseInOut(double? durationMs, double? delayMs = null)
        => new EaseInOutSpecification(TimeSpan.FromMilliseconds(durationMs ?? 200), TimeSpan.FromMilliseconds(delayMs ?? 0));

	public readonly static Spec EaseInOut100ms = EaseInOut(TimeSpan.FromMilliseconds(100), delay: null);
	public readonly static Spec EaseInOut200ms = EaseInOut(TimeSpan.FromMilliseconds(200), delay: null);
	public readonly static Spec EaseInOut500ms = EaseInOut(TimeSpan.FromMilliseconds(500), delay: null);
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
