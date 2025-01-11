using System.Net.NetworkInformation;

namespace BlazorCssTransitions;

public partial class Spec
{
    public static Spec EaseIn(TimeSpan? duration = null, TimeSpan? delay = null)
        => new EaseInSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
    public static Spec EaseIn(double? durationMs, double? delayMs = null)
        => new EaseInSpecification(TimeSpan.FromMilliseconds(durationMs ?? 200), TimeSpan.FromMilliseconds(delayMs ?? 0));

	public readonly static Spec EaseIn100ms = EaseIn(TimeSpan.FromMilliseconds(100), delay: null);
	public readonly static Spec EaseIn200ms = EaseIn(TimeSpan.FromMilliseconds(200), delay: null);
	public readonly static Spec EaseIn500ms = EaseIn(TimeSpan.FromMilliseconds(500), delay: null);
}

internal class EaseInSpecification : Spec
{
    public EaseInSpecification(TimeSpan duration, TimeSpan delay)
    {
        _duration = duration;
        _delay = delay;
        _timingFunction = "ease-in";
    }
}
