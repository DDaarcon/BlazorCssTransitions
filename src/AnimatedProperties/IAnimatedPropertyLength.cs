using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertyLength : IAnimatedPropertySettings<IAnimatedPropertyLength, IAnimatedPropertyLengthReady>
{
	IAnimatedPropertyLength SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, CssLength>> steps);
	IAnimatedPropertyLength Duplicate();
}

public interface IAnimatedPropertyLengthReady : IAnimatedPropertyLength, IAnimatedPropertyReadyToRegister
{
	IAnimatedPropertyLengthReady Duplicate();
}
