﻿namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

internal sealed class SlideInEnterTransition : BaseSpecificEnterTransition
{
    public SlideInEnterTransition(Spec spec, CssLengthPercentage initialOffsetX, CssLengthPercentage initialOffsetY)
    {
        TransitionAllTimeStyle = $"--start-slide-in-offset-x: {initialOffsetX};--start-slide-in-offset-y: {initialOffsetY};";
        TransitionInitialClass = _initialClass;
        TransitionFinishClass = _finishClass;
        TransitionedProperty = "translate";
        Specification = spec;
    }

    private const string _initialClass = "slide-in-animation-start";
    private const string _finishClass = "slide-in-animation-finish";

    protected override BaseSpecificEnterTransition ShallowClone()
        => (BaseSpecificEnterTransition)MemberwiseClone();
}
