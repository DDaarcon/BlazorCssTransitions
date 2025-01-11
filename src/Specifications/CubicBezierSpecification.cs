using BlazorCssTransitions.Shared;

namespace BlazorCssTransitions;

public partial class Spec
{
    public static Spec CubicBezier(float p1, float p2, float p3, float p4, TimeSpan? duration = null, TimeSpan? delay = null)
        => new CubicBezierSpecification(p1, p2, p3, p4, duration ?? TimeSpan.FromMilliseconds(200), delay ?? TimeSpan.Zero);
    public static Spec CubicBezier(float p1, float p2, float p3, float p4, double? durationMs, double? delayMs = null)
        => new CubicBezierSpecification(p1, p2, p3, p4, TimeSpan.FromMilliseconds(durationMs ?? 200), TimeSpan.FromMilliseconds(delayMs ?? 0));
}

internal class CubicBezierSpecification : Spec
{
    public CubicBezierSpecification(float p1, float p2, float p3, float p4, TimeSpan duration, TimeSpan delay)
    {
        _duration = duration;
        _delay = delay;
        _timingFunction = $"cubic-bezier({p1.ToCss()}, {p2.ToCss()}, {p3.ToCss()}, {p4.ToCss()})";
    }
}
