namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

internal class CombinedExitTransition : BaseCombinedTransition<ExitTransition, BaseSpecificExitTransition>, ExitTransition
{
    public CombinedExitTransition(IEnumerable<BaseSpecificExitTransition> transitions) : base(transitions)
    { }

    public CombinedExitTransition(ExitTransition firstTransition, ExitTransition secondTransition) : base(firstTransition, secondTransition)
    { }

    public ExitTransition CloneWith(Func<Spec, Spec> specTranformer)
    {
        return new CombinedExitTransition(_transitions.Select(x => (BaseSpecificExitTransition)x.CloneWith(specTranformer)));
    }

    public ExitTransition CombineWith(ExitTransition anotherTransition)
    {
        var newTransitionCollection = _transitions.Concat(GetTransitionsCollectionFromTransition(anotherTransition));

        return new CombinedExitTransition(newTransitionCollection);
    }
}
