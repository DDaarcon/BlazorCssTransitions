﻿namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertyLengthPercentage : IAnimatedPropertySettings<IAnimatedPropertyLengthPercentage, IAnimatedPropertyLengthPercentageReady>
{
	IAnimatedPropertyLengthPercentage SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, CssLengthPercentage>> steps);
	IAnimatedPropertyLengthPercentage Duplicate();
}

public interface IAnimatedPropertyLengthPercentageReady : IAnimatedPropertyLengthPercentage, IAnimatedPropertyReadyToRegister
{
	IAnimatedPropertyLengthPercentageReady Duplicate();
}