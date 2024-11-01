using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertyLength : IAnimatedPropertySettings<IAnimatedPropertyLength, IAnimatedPropertyLengthReady, IAnimatedPropertyLengthRegistration>
{
	IAnimatedPropertyLength SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, CssLength>> steps);
	IAnimatedPropertyLength Duplicate();
}

public interface IAnimatedPropertyLengthReady : IAnimatedPropertyLength, IAnimatedPropertyReadyToRegister<IAnimatedPropertyLengthRegistration>
{
	IAnimatedPropertyLengthReady Duplicate();
}

public interface IAnimatedPropertyLengthRegistration : IAnimatedPropertyRegistration
{
	Task<CssLength> GetCurrentValue();
	IAnimatedPropertyLengthFields State { get; }
}

public interface IAnimatedPropertyLengthFields : IAnimatedPropertyFields
{
    CssLength InitialValueLength { get; }
    CssLength FinalValueLength { get; }
}
