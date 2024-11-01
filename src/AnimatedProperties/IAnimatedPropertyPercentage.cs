namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertyPercentage : IAnimatedPropertySettings<IAnimatedPropertyPercentage, IAnimatedPropertyPercentageReady>
{
	IAnimatedPropertyPercentage SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, CssPercentage>> steps);
	IAnimatedPropertyPercentage Duplicate();
}

public interface IAnimatedPropertyPercentageReady : IAnimatedPropertyPercentage, IAnimatedPropertyReadyToRegister
{
	IAnimatedPropertyPercentageReady Duplicate();
}