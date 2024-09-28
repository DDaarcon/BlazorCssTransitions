using BlazorCssTransitions.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;
internal abstract class BaseSpecificExitTransition : BaseSpecificTransition<ExitTransition>, ExitTransition
{
    public ExitTransition CloneWith(Func<Specification, Specification> specTranformer)
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
