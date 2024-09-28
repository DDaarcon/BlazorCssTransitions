using BlazorCssAnimations.Specifications;

namespace BlazorCssAnimations.AnimatedVisibilityTransitions;

public interface EnterTransition : IBaseTransition
{
    public static EnterTransition FadeIn(Specification? spec = null, float initialOpacity = 0f, float finishOpacity = 1f)
        => new FadeInEnterTransition(
            spec ?? Specification.Linear(TimeSpan.FromSeconds(1)),
            initialOpacity,
            finishOpacity);

    public static EnterTransition SlideIn(Specification? spec = null, string initialX = "-100%", string initialY = "0")
        => new SlideInEnterTransition(
            spec ?? Specification.Linear(TimeSpan.FromSeconds(1)),
            initialX ?? "-100%",
            initialY ?? "0");

    public static EnterTransition ExpandVertically(Specification? spec = null)
        => new ExpandEnterTransition(
            spec ?? Specification.Linear(TimeSpan.FromSeconds(1)),
            startScaleX: 1,
            startScaleY: 0,
            finishScaleX: 1,
            finishScaleY: 1);

    public static EnterTransition ExpandHorizontally(Specification? spec = null)
        => new ExpandEnterTransition(
            spec ?? Specification.Linear(TimeSpan.FromSeconds(1)),
            startScaleX: 0,
            startScaleY: 1,
            finishScaleX: 1,
            finishScaleY: 1);

    public static EnterTransition operator +(EnterTransition firstTransition, EnterTransition secondTransition)
    {
        return firstTransition.CombineWith(secondTransition);
    }

    public EnterTransition CombineWith(EnterTransition anotherTransition);

    public EnterTransition CloneWith(Func<Specification, Specification> specTranformer);
}
