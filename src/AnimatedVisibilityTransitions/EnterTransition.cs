using BlazorCssTransitions.Specifications;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

public interface EnterTransition : IBaseTransition
{
    public static EnterTransition FadeIn(Spec? spec = null, float initialOpacity = 0f, float finishOpacity = 1f)
        => new FadeInEnterTransition(
            spec ?? Spec.Linear(),
            initialOpacity,
            finishOpacity);


    public static EnterTransition SlideIn(Spec? spec = null, string initialX = "-100%", string initialY = "0")
        => new SlideInEnterTransition(
            spec ?? Spec.Linear(),
            initialX,
            initialY);
    public static EnterTransition SlideInVertically(Spec? spec = null, string initialY = "100%")
        => new SlideInEnterTransition(
            spec ?? Spec.Linear(),
            "0",
            initialY);
    public static EnterTransition SlideInHorizontally(Spec? spec = null, string initialX = "-100%")
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
