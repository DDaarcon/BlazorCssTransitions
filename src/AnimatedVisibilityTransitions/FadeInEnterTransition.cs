using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

internal sealed class FadeInEnterTransition : BaseSpecificEnterTransition
{
    public FadeInEnterTransition(Specification spec, float initialOpacity, float finishOpacity)
    {
        TransitionAllTimeStyle = $"""
            --start-fade-in-opacity: {initialOpacity.ToCss()};
            --finish-fade-in-opacity: {finishOpacity.ToCss()};
            """;
        TransitionInitialClass = _initialClass;
        TransitionFinishClass = _finishClass;
        Specification = spec;
        TransitionedProperty = "opacity";
    }

    private const string _initialClass = "fade-in-animation-start";
    private const string _finishClass = "fade-in-animation-finish";

    protected override BaseSpecificEnterTransition ShallowClone()
        => (BaseSpecificEnterTransition)MemberwiseClone();
}
