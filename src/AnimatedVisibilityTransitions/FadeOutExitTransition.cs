using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

internal class FadeOutExitTransition : BaseSpecificExitTransition
{
    public FadeOutExitTransition(Specification spec, float initialOpacity, float finishOpacity)
    {
        TransitionAllTimeStyle = $"""
            --start-fade-out-opacity: {initialOpacity.ToCss()};
            --finish-fade-out-opacity: {finishOpacity.ToCss()};
            """;
        TransitionInitialClass = _initialClass;
        TransitionFinishClass = _finishClass;
        Specification = spec;
        TransitionedProperty = "opacity";
    }

    private const string _initialClass = "fade-out-animation-start";
    private const string _finishClass = "fade-out-animation-finish";

    protected override BaseSpecificExitTransition ShallowClone()
        => (BaseSpecificExitTransition)MemberwiseClone();
}
