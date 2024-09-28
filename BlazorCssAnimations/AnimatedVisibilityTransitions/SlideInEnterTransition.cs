using BlazorCssAnimations.Specifications;

namespace BlazorCssAnimations.AnimatedVisibilityTransitions;

internal sealed class SlideInEnterTransition : BaseSpecificEnterTransition
{
    public SlideInEnterTransition(Specification spec, string initialTransitionX, string initialTransitionY)
    {
        TransitionAllTimeStyle = $"""
            --start-offset-x: {initialTransitionX};
            --start-offset-y: {initialTransitionY};
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
