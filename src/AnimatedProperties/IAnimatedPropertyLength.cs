namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertyLength : IAnimatedPropertySettings<IAnimatedPropertyLength, IAnimatedPropertyLengthReady, IAnimatedPropertyLengthRegistration>
{
	IAnimatedPropertyLength SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, CssLength>> steps);
	IAnimatedPropertyLength Duplicate();
}

public interface IAnimatedPropertyLengthReady : IAnimatedPropertyLength, IAnimatedPropertyReadyToRegister<IAnimatedPropertyLengthRegistration>
{
	new IAnimatedPropertyLengthReady Duplicate();
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
