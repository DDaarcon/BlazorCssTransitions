namespace BlazorCssAnimations.Specifications;

public partial class Specification
{
    public static Specification EaseOut(TimeSpan? duration = null, TimeSpan? delay = null)
        => new EaseOutSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
}

internal class EaseOutSpecification : Specification
{
    public EaseOutSpecification(TimeSpan duration, TimeSpan delay)
    {
        _duration = duration;
        _delay = delay;
        _timingFunction = "ease-out";
    }
}
