using System.Drawing;

namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertyColor : IAnimatedPropertySettings<IAnimatedPropertyColor, IAnimatedPropertyColorReady, IAnimatedPropertyColorRegistration>
{
	IAnimatedPropertyColor SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, Color>> steps);
	IAnimatedPropertyColor Duplicate();
}

public interface IAnimatedPropertyColorReady : IAnimatedPropertyColor, IAnimatedPropertyReadyToRegister<IAnimatedPropertyColorRegistration>
{
	new IAnimatedPropertyColorReady Duplicate();
}

public interface IAnimatedPropertyColorRegistration : IAnimatedPropertyRegistration
{
	Task<Color?> GetCurrentValue();
	IAnimatedPropertyColorFields State { get; }
}

public interface IAnimatedPropertyColorFields : IAnimatedPropertyFields
{
    Color InitialValueColor { get; }
    Color FinalValueColor { get; }
}
