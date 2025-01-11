using BlazorCssTransitions.Shared;

namespace BlazorCssTransitions;

public partial class Spec
{
    public static Spec Ease(TimeSpan? duration = null, TimeSpan? delay = null)
        => new EaseSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
    public static Spec Ease(double? durationMs, double? delayMs = null)
        => new EaseSpecification(TimeSpan.FromMilliseconds(durationMs ?? 200), TimeSpan.FromMilliseconds(delayMs ?? 0));

	public readonly static Spec Ease100ms = Ease(TimeSpan.FromMilliseconds(100), delay: null);
	public readonly static Spec Ease200ms = Ease(TimeSpan.FromMilliseconds(200), delay: null);
	public readonly static Spec Ease500ms = Ease(TimeSpan.FromMilliseconds(500), delay: null);
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
