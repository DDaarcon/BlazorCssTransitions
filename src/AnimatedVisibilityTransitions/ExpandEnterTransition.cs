using BlazorCssTransitions.Shared;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

internal class ExpandEnterTransition : BaseSpecificEnterTransition
{
    public ExpandEnterTransition(Spec spec, float startScaleX, float startScaleY, float finishScaleX, float finishScaleY)
    {
        TransitionAllTimeStyle = $"--start-expand-scale-x: {startScaleX.ToCss()};--start-expand-scale-y: {startScaleY.ToCss()};--finish-expand-scale-x: {finishScaleX.ToCss()};--finish-expand-scale-y: {finishScaleY.ToCss()};";
        TransitionInitialClass = _initialClass;
        TransitionFinishClass = _finishClass;
        Specification = spec;
        TransitionedProperty = "scale";
    }

    private const string _initialClass = "expand-animation-start";
    private const string _finishClass = "expand-animation-finish";

    protected override BaseSpecificEnterTransition ShallowClone()
        => (BaseSpecificEnterTransition)MemberwiseClone();
}
