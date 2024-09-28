namespace BlazorCssAnimations.Specifications;

public partial class Specification
{
    public static Specification EaseInOut(TimeSpan? duration = null, TimeSpan? delay = null)
        => new EaseInOutSpecification(duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
}

internal class EaseInOutSpecification : Specification
{
    public EaseInOutSpecification(TimeSpan duration, TimeSpan delay)
    {
        _duration = duration;
        _delay = delay;
        _timingFunction = "ease-in-out";
    }
}
