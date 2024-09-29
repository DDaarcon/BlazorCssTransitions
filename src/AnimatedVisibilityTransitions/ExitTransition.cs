using BlazorCssTransitions.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

public interface ExitTransition : IBaseTransition
{
    public static ExitTransition FadeOut(Specification? spec = null, float initialOpacity = 1f, float finishOpacity = 0f)
        => new FadeOutExitTransition(
            spec ?? Specification.Linear(),
            initialOpacity,
            finishOpacity);

    public static ExitTransition operator +(ExitTransition firstTransition, ExitTransition secondTransition)
    {
        return firstTransition.CombineWith(secondTransition);
    }

    public ExitTransition CombineWith(ExitTransition anotherTransition);

    public ExitTransition CloneWith(Func<Specification, Specification> specTranformer);
}
