using BlazorCssTransitions.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.Specifications;

// TODO remove unused properties
public partial class Spec
{
    protected Spec() { }

    protected TimeSpan? _duration { get; init; }
    protected TimeSpan? _delay { get; init; }
    protected string? _timingFunction { get; init; }

    internal TimeSpan Duration => Assertions.AssertNotNullAndGet(_duration, "Transition's duration must be set before transition is used.");
    internal TimeSpan Delay => Assertions.AssertNotNullAndGet(_delay, "Transition's delay must be set before transition is used.");
    internal string TimingFunction => Assertions.AssertNotNullAndGet(_timingFunction, "Transition's timing function must be set before transition is used.");

    /// <summary>
    /// Returns: [animated-prop] [duration] [delay] [riming-func]
    /// </summary>
    internal virtual string GetTransitionValue(string animatedProperty)
        => $"{animatedProperty} {Duration.ToCssTime()} {Delay.ToCssTime()} {TimingFunction}";

    /// <summary>
    /// Returns while property declaration for "transition"
    /// </summary>
    internal string GetStyle(string animatedProperty)
    {
        return $"transition: {animatedProperty} {Duration.ToCssTime()} {Delay.ToCssTime()} {TimingFunction};";
    }

    /// <summary>
    /// Returns: [duration] [timing-func] [delay]
    /// </summary>
    internal virtual string GetAnimationValue()
        => $"{Duration.ToCssTime()} {TimingFunction} {Delay.ToCssTime()}";

    public Spec CloneWith(TimeSpan? newDuration, TimeSpan? newDelay)
    {
        return new Spec
        {
            _duration = newDuration ?? Duration,
            _delay = newDelay ?? Delay,
            _timingFunction = TimingFunction
        };
    }
}
