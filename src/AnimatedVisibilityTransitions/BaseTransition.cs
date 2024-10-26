using BlazorCssTransitions.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;
internal abstract class BaseTransition : IBaseTransition
{
    abstract internal string GetInitialStyle();
    abstract internal string GetFinishStyle();
    abstract internal string GetFinishedStyle();
    abstract internal string GetInitialClasses();
    abstract internal string GetFinishClasses();

    abstract internal IEnumerable<Spec> GetSpecifications();
}
