using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertyColor : IAnimatedPropertySettings<IAnimatedPropertyColor, IAnimatedPropertyColorReady, IAnimatedPropertyColorRegistration>
{
	IAnimatedPropertyColor SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, Color>> steps);
	IAnimatedPropertyColor Duplicate();
}

public interface IAnimatedPropertyColorReady : IAnimatedPropertyColor, IAnimatedPropertyReadyToRegister<IAnimatedPropertyColorRegistration>
{
	IAnimatedPropertyColorReady Duplicate();
}

public interface IAnimatedPropertyColorRegistration : IAnimatedPropertyRegistration
{
	Task<Color?> GetCurrentValue();
}
