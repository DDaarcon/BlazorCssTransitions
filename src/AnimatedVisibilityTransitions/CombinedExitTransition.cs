using BlazorCssTransitions.Specifications;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

internal class CombinedExitTransition : BaseCombinedTransition<ExitTransition, BaseSpecificExitTransition>, ExitTransition
{
    public CombinedExitTransition(IEnumerable<BaseSpecificExitTransition> transitions) : base(transitions)
    { }

    public CombinedExitTransition(ExitTransition firstTransition, ExitTransition secondTransition) : base(firstTransition, secondTransition)
    { }

    public ExitTransition CloneWith(Func<Specification, Specification> specTranformer)
    {
        return new CombinedExitTransition(_transitions.Select(x => (BaseSpecificExitTransition)x.CloneWith(specTranformer)));
    }

    public ExitTransition CombineWith(ExitTransition anotherTransition)
    {
        var newTransitionCollection = _transitions.Concat(GetTransitionsCollectionFromTransition(anotherTransition));

        return new CombinedExitTransition(newTransitionCollection);
    }
}
