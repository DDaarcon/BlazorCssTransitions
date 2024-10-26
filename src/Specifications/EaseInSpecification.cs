using System.Net.NetworkInformation;

namespace BlazorCssTransitions.Specifications;

public partial class Spec
{
    public static Spec EaseIn(TimeSpan? duration = null, TimeSpan? delay = null)
        => new EaseInSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
    public static Spec EaseIn(double? durationMs, double? delayMs = null)
        => new EaseInSpecification(TimeSpan.FromMilliseconds(durationMs ?? 200), TimeSpan.FromMilliseconds(delayMs ?? 0));
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
