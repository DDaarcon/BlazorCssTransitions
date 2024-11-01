namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertyPercentage : IAnimatedPropertySettings<IAnimatedPropertyPercentage, IAnimatedPropertyPercentageReady, IAnimatedPropertyPercentageRegistration>
{
	IAnimatedPropertyPercentage SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, CssPercentage>> steps);
	IAnimatedPropertyPercentage Duplicate();
}

public interface IAnimatedPropertyPercentageReady : IAnimatedPropertyPercentage, IAnimatedPropertyReadyToRegister<IAnimatedPropertyPercentageRegistration>
{
	IAnimatedPropertyPercentageReady Duplicate();
}

public interface IAnimatedPropertyPercentageRegistration : IAnimatedPropertyRegistration
{
	Task<CssPercentage> GetCurrentValue();
}