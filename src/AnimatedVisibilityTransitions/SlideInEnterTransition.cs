using BlazorCssTransitions.Specifications;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

internal sealed class SlideInEnterTransition : BaseSpecificEnterTransition
{
    public SlideInEnterTransition(Specification spec, string initialOffsetX, string initialOffsetY)
    {
        TransitionAllTimeStyle = $"""
            --start-slide-in-offset-x: {initialOffsetX};
            --start-slide-in-offset-y: {initialOffsetY};
            """;
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
