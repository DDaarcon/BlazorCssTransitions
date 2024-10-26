using BlazorCssTransitions.Specifications;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

internal class SlideOutExitTransition : BaseSpecificExitTransition
{
    public SlideOutExitTransition(Spec spec, string finishOffsetX, string finishOffsetY)
    {
        TransitionAllTimeStyle = $"""
            --finish-slide-out-offset-x: {finishOffsetX};
            --finish-slide-out-offset-y: {finishOffsetY};
            """;
        TransitionInitialClass = _initialClass;
        TransitionFinishClass = _finishClass;
        TransitionedProperty = "translate";
        Specification = spec;
    }

    private const string _initialClass = "slide-out-animation-start";
    private const string _finishClass = "slide-out-animation-finish";

    protected override BaseSpecificExitTransition ShallowClone()
        => (BaseSpecificExitTransition)MemberwiseClone();
}
