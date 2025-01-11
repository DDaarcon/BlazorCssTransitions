using BlazorCssTransitions.AnimatedVisibilityTransitions;

namespace BlazorCssTransitions;

#pragma warning disable IDE1006 // Naming Styles
public interface ExitTransition : IBaseTransition
#pragma warning restore IDE1006 // Naming Styles
{
    public static ExitTransition FadeOut(Spec? spec = null, float initialOpacity = 1f, float finishOpacity = 0f)
        => new FadeOutExitTransition(
            spec ?? Spec.Linear(),
            initialOpacity,
            finishOpacity);

    public static ExitTransition SlideOut(Spec? spec = null, string finishX = "-100%", string finishY = "0")
        => SlideOut(spec, new CssLengthPercentage(finishX), new CssLengthPercentage(finishY));
    public static ExitTransition SlideOut(Spec? spec, CssLengthPercentage finishX, CssLengthPercentage finishY)
        => new SlideOutExitTransition(
            spec ?? Spec.Linear(),
            finishX,
            finishY);

    public static ExitTransition SlideOutVertically(Spec? spec = null, string finishY = "100%")
        => SlideOutVertically(spec, new CssLengthPercentage(finishY));
    public static ExitTransition SlideOutVertically(Spec? spec, CssLengthPercentage finishY)
        => new SlideOutExitTransition(
            spec ?? Spec.Linear(),
            "0",
            finishY);

    public static ExitTransition SlideOutHorizontally(Spec? spec = null, string finishX = "-100%")
        => SlideOutHorizontally(spec, new CssLengthPercentage(finishX));
    public static ExitTransition SlideOutHorizontally(Spec? spec, CssLengthPercentage finishX)
        => new SlideOutExitTransition(
            spec ?? Spec.Linear(),
            finishX,
            "0");


    public static ExitTransition operator +(ExitTransition firstTransition, ExitTransition secondTransition)
    {
        return firstTransition.CombineWith(secondTransition);
    }

    public ExitTransition CombineWith(ExitTransition anotherTransition);

    public ExitTransition CloneWith(Func<Spec, Spec> specTranformer);
}
