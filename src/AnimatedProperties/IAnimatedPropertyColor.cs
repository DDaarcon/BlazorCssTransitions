using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertyColor : IAnimatedPropertySettings<IAnimatedPropertyColor, IAnimatedPropertyColorReady>
{
	IAnimatedPropertyColor SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, Color>> steps);
	IAnimatedPropertyColor Duplicate();
}

public interface IAnimatedPropertyColorReady : IAnimatedPropertyColor, IAnimatedPropertyReadyToRegister
{
	IAnimatedPropertyColorReady Duplicate();
}
