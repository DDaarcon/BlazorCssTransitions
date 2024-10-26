using BlazorCssTransitions.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

public interface ExitTransition : IBaseTransition
{
    public static ExitTransition FadeOut(Spec? spec = null, float initialOpacity = 1f, float finishOpacity = 0f)
        => new FadeOutExitTransition(
            spec ?? Spec.Linear(),
            initialOpacity,
            finishOpacity);

    public static ExitTransition SlideOut(Spec? spec = null, string finishX = "-100%", string finishY = "0")
        => new SlideOutExitTransition(
            spec ?? Spec.Linear(),
            finishX,
            finishY);

    public static ExitTransition SlideOutVertically(Spec? spec = null, string finishY = "100%")
        => new SlideOutExitTransition(
            spec ?? Spec.Linear(),
            "0",
            finishY);

    public static ExitTransition SlideOutHorizontally(Spec? spec = null, string finishX = "-100%")
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
