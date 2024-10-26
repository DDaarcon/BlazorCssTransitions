using BlazorCssTransitions.Specifications;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

internal sealed class CombinedEnterTransition : BaseCombinedTransition<EnterTransition, BaseSpecificEnterTransition>, EnterTransition
{
    public CombinedEnterTransition(IEnumerable<BaseSpecificEnterTransition> transitions) : base(transitions)
    { }

    public CombinedEnterTransition(EnterTransition firstTransition, EnterTransition secondTransition) : base(firstTransition, secondTransition)
    { }

    public EnterTransition CloneWith(Func<Spec, Spec> specTranformer)
    {
        return new CombinedEnterTransition(_transitions.Select(x => (BaseSpecificEnterTransition)x.CloneWith(specTranformer)));
    }

    public EnterTransition CombineWith(EnterTransition anotherTransition)
    {
        var newTransitionCollection = _transitions.Concat(GetTransitionsCollectionFromTransition(anotherTransition));

        return new CombinedEnterTransition(newTransitionCollection);
    }
}
