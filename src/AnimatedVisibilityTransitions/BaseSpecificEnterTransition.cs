using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Specifications;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

internal abstract class BaseSpecificEnterTransition : BaseSpecificTransition<EnterTransition>, EnterTransition
{
    public EnterTransition CloneWith(Func<Spec, Spec> specTranformer)
    {
        var clone = ShallowClone();
        clone.Specification = specTranformer(clone.Specification);
        return clone;
    }

    public EnterTransition CombineWith(EnterTransition anotherTransition)
    {
        return new CombinedEnterTransition(this, anotherTransition);
    }

    protected abstract BaseSpecificEnterTransition ShallowClone();
}
