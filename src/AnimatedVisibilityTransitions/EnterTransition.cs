using BlazorCssTransitions.AnimatedVisibilityTransitions;

namespace BlazorCssTransitions;

#pragma warning disable IDE1006 // Naming Styles
public interface EnterTransition : IBaseTransition
#pragma warning restore IDE1006 // Naming Styles
{
    public static EnterTransition FadeIn(Spec? spec = null, float initialOpacity = 0f, float finishOpacity = 1f)
        => new FadeInEnterTransition(
            spec ?? Spec.Linear(),
            initialOpacity,
            finishOpacity);


    public static EnterTransition SlideIn(Spec? spec = null, string initialX = "-100%", string initialY = "0")
        => SlideIn(spec, new CssLengthPercentage(initialX), new CssLengthPercentage(initialY));
    public static EnterTransition SlideIn(Spec? spec, CssLengthPercentage initialX, CssLengthPercentage initialY)
        => new SlideInEnterTransition(
            spec ?? Spec.Linear(),
            initialX,
            initialY);

    public static EnterTransition SlideInVertically(Spec? spec = null, string initialY = "100%")
        => SlideInVertically(spec, new CssLengthPercentage(initialY));
    public static EnterTransition SlideInVertically(Spec? spec, CssLengthPercentage initialY)
		=> new SlideInEnterTransition(
			spec ?? Spec.Linear(),
			"0",
			initialY);

    public static EnterTransition SlideInHorizontally(Spec? spec = null, string initialX = "-100%")
        => SlideInHorizontally(spec, new CssLengthPercentage(initialX));
	public static EnterTransition SlideInHorizontally(Spec? spec, CssLengthPercentage initialX)
        => new SlideInEnterTransition(
            spec ?? Spec.Linear(),
            initialX,
            "0");


    public static EnterTransition ExpandVertically(Spec? spec = null)
        => new ExpandEnterTransition(
            spec ?? Spec.Linear(),
            startScaleX: 1,
            startScaleY: 0,
            finishScaleX: 1,
            finishScaleY: 1);
    public static EnterTransition ExpandHorizontally(Spec? spec = null)
        => new ExpandEnterTransition(
            spec ?? Spec.Linear(),
            startScaleX: 0,
            startScaleY: 1,
            finishScaleX: 1,
            finishScaleY: 1);

    public static EnterTransition operator +(EnterTransition firstTransition, EnterTransition secondTransition)
    {
        return firstTransition.CombineWith(secondTransition);
    }

    public EnterTransition CombineWith(EnterTransition anotherTransition);

    public EnterTransition CloneWith(Func<Spec, Spec> specTranformer);
}
