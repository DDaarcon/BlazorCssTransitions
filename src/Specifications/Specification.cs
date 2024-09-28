using BlazorCssTransitions.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.Specifications;

// TODO remove unused properties
public partial class Specification
{
    protected Specification() { }

    protected TimeSpan? _duration;
    protected TimeSpan? _delay;
    protected string? _timingFunction;

    internal TimeSpan Duration => Assertions.AssertNotNullAndGet(ref _duration, "Transition's duration must be set before transition is used.");
    internal TimeSpan Delay => Assertions.AssertNotNullAndGet(ref _delay, "Transition's delay must be set before transition is used.");
    internal string TimingFunction => Assertions.AssertNotNullAndGet(ref _timingFunction, "Transition's timing function must be set before transition is used.");

    internal virtual string GetTransitionValue(string animatedProperty)
        => $"{animatedProperty} {Duration.ToCssDuration()} {Delay.ToCssDuration()} {TimingFunction}";

    internal string GetStyle(string animatedProperty)
    {
        return $"transition: {animatedProperty} {Duration.ToCssDuration()} {Delay.ToCssDuration()} {TimingFunction};";
    }

    public Specification CloneWith(TimeSpan? newDuration, TimeSpan? newDelay)
    {
        return new Specification
        {
            _duration = newDuration ?? Duration,
            _delay = newDelay ?? Delay,
            _timingFunction = TimingFunction
        };
    }
}
