using BlazorCssTransitions.Help;

namespace BlazorCssTransitions.Specifications;

public partial class Specification
{
    public static Specification Ease(TimeSpan? duration = null, TimeSpan? delay = null)
        => new EaseSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
}

internal class EaseSpecification : Specification
{
    public EaseSpecification(TimeSpan duration, TimeSpan delay)
    {
        _duration = duration;
        _delay = delay;
        _timingFunction = "ease";
    }
}
