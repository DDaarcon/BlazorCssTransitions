using System.Net.NetworkInformation;

namespace BlazorCssTransitions.Specifications;

public partial class Specification
{
    public static Specification EaseIn(TimeSpan? duration = null, TimeSpan? delay = null)
        => new EaseInSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
}

internal class EaseInSpecification : Specification
{
    public EaseInSpecification(TimeSpan duration, TimeSpan delay)
    {
        _duration = duration;
        _delay = delay;
        _timingFunction = "ease-in";
    }
}
