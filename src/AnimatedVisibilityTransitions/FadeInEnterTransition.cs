using BlazorCssTransitions.Shared;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

internal sealed class FadeInEnterTransition : BaseSpecificEnterTransition
{
    public FadeInEnterTransition(Spec spec, float initialOpacity, float finishOpacity)
    {
        TransitionAllTimeStyle = $"--start-fade-in-opacity: {initialOpacity.ToCss()};--finish-fade-in-opacity: {finishOpacity.ToCss()};";
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
