namespace BlazorCssTransitions.AnimatedVisibilityTransitions;
internal abstract class BaseSpecificExitTransition : BaseSpecificTransition<ExitTransition>, ExitTransition
{
    public ExitTransition CloneWith(Func<Spec, Spec> specTranformer)
    {
        var clone = ShallowClone();
        clone.Specification = specTranformer(clone.Specification);
        return clone;
    }

    public ExitTransition CombineWith(ExitTransition anotherTransition)
    {
        return new CombinedExitTransition(this, anotherTransition);
    }

    protected abstract BaseSpecificExitTransition ShallowClone();
}
