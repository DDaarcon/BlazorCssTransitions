using System.Drawing;

namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertyLengthPercentage : IAnimatedPropertySettings<IAnimatedPropertyLengthPercentage, IAnimatedPropertyLengthPercentageReady, IAnimatedPropertyLengthPercentageRegistration>
{
	IAnimatedPropertyLengthPercentage SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, CssLengthPercentage>> steps);
	IAnimatedPropertyLengthPercentage Duplicate();
}

public interface IAnimatedPropertyLengthPercentageReady : IAnimatedPropertyLengthPercentage, IAnimatedPropertyReadyToRegister<IAnimatedPropertyLengthPercentageRegistration>
{
	IAnimatedPropertyLengthPercentageReady Duplicate();
}

public interface IAnimatedPropertyLengthPercentageRegistration : IAnimatedPropertyRegistration
{
	Task<CssLengthPercentage> GetCurrentValue();
    IAnimatedPropertyLengthPercentageFields State { get; }
}

public interface IAnimatedPropertyLengthPercentageFields : IAnimatedPropertyFields
{
    CssLengthPercentage InitialValueLengthPercentage { get; }
    CssLengthPercentage FinalValueLengthPercentage { get; }
}