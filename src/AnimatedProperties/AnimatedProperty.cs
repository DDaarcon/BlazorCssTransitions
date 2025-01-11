using BlazorCssTransitions.Shared;
using System.Drawing;

namespace BlazorCssTransitions.AnimatedProperties;
using IColorSettings = IAnimatedPropertySettings<IAnimatedPropertyColor, IAnimatedPropertyColorReady, IAnimatedPropertyColorRegistration>;
using ILengthSettings = IAnimatedPropertySettings<IAnimatedPropertyLength, IAnimatedPropertyLengthReady, IAnimatedPropertyLengthRegistration>;
using IPercentageSettings = IAnimatedPropertySettings<IAnimatedPropertyPercentage, IAnimatedPropertyPercentageReady, IAnimatedPropertyPercentageRegistration>;
using ILengthPercentageSettings = IAnimatedPropertySettings<IAnimatedPropertyLengthPercentage, IAnimatedPropertyLengthPercentageReady, IAnimatedPropertyLengthPercentageRegistration>;


internal class AnimatedProperty 
	: IAnimatedPropertyColor, IAnimatedPropertyColorReady, IAnimatedPropertyColorFields,
		IAnimatedPropertyLength, IAnimatedPropertyLengthReady, IAnimatedPropertyLengthFields,
		IAnimatedPropertyPercentage, IAnimatedPropertyPercentageReady, IAnimatedPropertyPercentageFields,
		IAnimatedPropertyLengthPercentage, IAnimatedPropertyLengthPercentageReady, IAnimatedPropertyLengthPercentageFields
{
	private readonly AnimatedPropertiesCreatorImpl _creator;
	private readonly Func<string, Task<(AnimatedPropertiesCreatorImpl.ReadStylePropertyResult, string?)>> _readStyleProperty;

	internal AnimatedProperty(
		AnimatedPropertiesCreatorImpl creator,
		Func<string, Task<(AnimatedPropertiesCreatorImpl.ReadStylePropertyResult, string?)>> readStyleProperty,
		PropertySyntax syntax,
		string initialValue,
		string finalValue)
	{
		_creator = creator;
		_syntax = syntax;
		_initialValue = initialValue;
		_finalValue = finalValue;
		_readStyleProperty = readStyleProperty;
	}

	private readonly string _name = "a" + Guid.NewGuid().ToString();

	private readonly PropertySyntax _syntax;
	private readonly string _initialValue;

	private readonly string _finalValue;

	private Spec? _spec;
	private int _iterationCount = DefaultIterationCount;
	private AnimationDirection _direction = DefaultDirection;
	private AnimationFillMode _fillMode = DefaultFillMode;
	private IEnumerable<KeyValuePair<CssPercentage, string>>? _intermediateStates;

	internal const int DefaultIterationCount = 1;
	internal const AnimationDirection DefaultDirection = AnimationDirection.Normal;
	internal const AnimationFillMode DefaultFillMode = AnimationFillMode.Backwards;

	private bool _isRunning = true;


	public string Name => _name;
	internal string AnimationName => _name + "-anim";
	public PropertySyntax Syntax => _syntax;
	public string InitialValue => _initialValue;
	public string FinalValue => _finalValue;
	public IEnumerable<KeyValuePair<CssPercentage, string>>? IntermediateStates => _intermediateStates;
	private Spec Spec => _spec ?? throw new ArgumentNullException("Specification (spec) must be provided to construct an animated property");
    Spec? IAnimatedPropertyFields.Spec => _spec;
    public int IterationCount => _iterationCount;
    public AnimationDirection Direction => _direction;
    public AnimationFillMode FillMode => _fillMode;
	public bool IsRunning => _isRunning;


    public Color InitialValueColor => _syntax is PropertySyntax.Color
		? ColorTranslator.FromHtml(InitialValue) : new Color();
    public Color FinalValueColor => _syntax is PropertySyntax.Color
        ? ColorTranslator.FromHtml(FinalValue) : new Color();
    public CssLength InitialValueLength => _syntax is PropertySyntax.Length
        ? InitialValue : CssLength.Unassigned();
    public CssLength FinalValueLength => _syntax is PropertySyntax.Length
        ? FinalValue : CssLength.Unassigned();
    public CssPercentage InitialValuePercentage => _syntax is PropertySyntax.Percentage
        ? InitialValue : CssPercentage.Unassigned();
    public CssPercentage FinalValuePercentage => _syntax is PropertySyntax.Percentage
        ? FinalValue : CssPercentage.Unassigned();
    public CssLengthPercentage InitialValueLengthPercentage => _syntax is PropertySyntax.LengthPercentage
        ? InitialValue : CssLengthPercentage.Unassigned();
    public CssLengthPercentage FinalValueLengthPercentage => _syntax is PropertySyntax.LengthPercentage
        ? FinalValue : CssLengthPercentage.Unassigned();



    internal string GetAnimationValue()
		=> $"{Spec.GetAnimationValue()} {IterationCountCss(_iterationCount)} {_direction.ToCss()} {_fillMode.ToCss()} running {AnimationName}";


	private static string IterationCountCss(int iterCount)
		=> iterCount is AnimatedPropertiesCreator.InfiniteIterationCount
			? "infinite"
			: iterCount.ToString();


	IAnimatedPropertyColorReady IColorSettings.WithSpec(Spec spec)
		=> FluentlySetSpec<IAnimatedPropertyColorReady>(spec);
	IAnimatedPropertyLengthReady ILengthSettings.WithSpec(Spec spec)
		=> FluentlySetSpec<IAnimatedPropertyLengthReady>(spec);
	IAnimatedPropertyPercentageReady IPercentageSettings.WithSpec(Spec spec)
		=> FluentlySetSpec<IAnimatedPropertyPercentageReady>(spec);
	IAnimatedPropertyLengthPercentageReady ILengthPercentageSettings.WithSpec(Spec spec)
		=> FluentlySetSpec<IAnimatedPropertyLengthPercentageReady>(spec);
	private TThisSpecialized FluentlySetSpec<TThisSpecialized>(Spec spec)
		where TThisSpecialized : class
		=> DoFluently<TThisSpecialized>(() => _spec = spec);


	IAnimatedPropertyColor IColorSettings.WithIterationCount(int count)
		=> FluentlySetIterationCount<IAnimatedPropertyColor>(count);
	IAnimatedPropertyLength ILengthSettings.WithIterationCount(int count)
		=> FluentlySetIterationCount<IAnimatedPropertyLength>(count);
	IAnimatedPropertyPercentage IPercentageSettings.WithIterationCount(int count)
		=> FluentlySetIterationCount<IAnimatedPropertyPercentage>(count);
	IAnimatedPropertyLengthPercentage ILengthPercentageSettings.WithIterationCount(int count)
		=> FluentlySetIterationCount<IAnimatedPropertyLengthPercentage>(count);
	private TThisSpecialized FluentlySetIterationCount<TThisSpecialized>(int count)
		where TThisSpecialized : class
		=> DoFluently<TThisSpecialized>(() => _iterationCount = count);


	IAnimatedPropertyColor IColorSettings.WithDirection(AnimationDirection direction)
		=> FluentlySetDirection<IAnimatedPropertyColor>(direction);
	IAnimatedPropertyLength ILengthSettings.WithDirection(AnimationDirection direction)
		=> FluentlySetDirection<IAnimatedPropertyLength>(direction);
	IAnimatedPropertyPercentage IPercentageSettings.WithDirection(AnimationDirection direction)
		=> FluentlySetDirection<IAnimatedPropertyPercentage>(direction);
	IAnimatedPropertyLengthPercentage ILengthPercentageSettings.WithDirection(AnimationDirection direction)
		=> FluentlySetDirection<IAnimatedPropertyLengthPercentage>(direction);
	private TThisSpecialized FluentlySetDirection<TThisSpecialized>(AnimationDirection direction)
		where TThisSpecialized : class
		=> DoFluently<TThisSpecialized>(() => _direction = direction);


	IAnimatedPropertyColor IColorSettings.WithFillMode(AnimationFillMode fillMode)
		=> FluentlySetFillMode<IAnimatedPropertyColor>(fillMode);
	IAnimatedPropertyLength ILengthSettings.WithFillMode(AnimationFillMode fillMode)
		=> FluentlySetFillMode<IAnimatedPropertyLength>(fillMode);
	IAnimatedPropertyPercentage IPercentageSettings.WithFillMode(AnimationFillMode fillMode)
		=> FluentlySetFillMode<IAnimatedPropertyPercentage>(fillMode);
	IAnimatedPropertyLengthPercentage ILengthPercentageSettings.WithFillMode(AnimationFillMode fillMode)
		=> FluentlySetFillMode<IAnimatedPropertyLengthPercentage>(fillMode);
	private TThisSpecialized FluentlySetFillMode<TThisSpecialized>(AnimationFillMode fillMode)
		where TThisSpecialized : class
		=> DoFluently<TThisSpecialized>(() => _fillMode = fillMode);


	IAnimatedPropertyColor IAnimatedPropertyColor.SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, Color>> steps)
		=> FluentlySetIntermediateSteps<IAnimatedPropertyColor, Color>(steps, value => value.ToHex());
	IAnimatedPropertyLength IAnimatedPropertyLength.SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, CssLength>> steps)
		=> FluentlySetIntermediateSteps<IAnimatedPropertyLength, CssLength>(steps, value => value.ToString());
	IAnimatedPropertyPercentage IAnimatedPropertyPercentage.SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, CssPercentage>> steps)
		=> FluentlySetIntermediateSteps<IAnimatedPropertyPercentage, CssPercentage>(steps, value => value.ToString());
	IAnimatedPropertyLengthPercentage IAnimatedPropertyLengthPercentage.SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, CssLengthPercentage>> steps)
		=> FluentlySetIntermediateSteps<IAnimatedPropertyLengthPercentage, CssLengthPercentage>(steps, value => value.ToString());
	private TThisSpecialized FluentlySetIntermediateSteps<TThisSpecialized, TValue>(IEnumerable<KeyValuePair<CssPercentage, TValue>> steps, Func<TValue, string> valueToString)
		where TThisSpecialized : class
		=> DoFluently<TThisSpecialized>(() =>
		{
			_intermediateStates = steps.Select(step =>
			{
				AssertPercentageBetween0and100Exclusive(step.Key);
				return new KeyValuePair<CssPercentage, string>(step.Key, valueToString(step.Value));
			});
		});


	IAnimatedPropertyColorReady IColorSettings.With(Spec spec, int iterCount, AnimationDirection direction, AnimationFillMode fillMode)
		=> FluentlySetAll<IAnimatedPropertyColorReady>(spec, iterCount, direction, fillMode);
	IAnimatedPropertyLengthReady ILengthSettings.With(Spec spec, int iterCount, AnimationDirection direction, AnimationFillMode fillMode)
		=> FluentlySetAll<IAnimatedPropertyLengthReady>(spec, iterCount, direction, fillMode);
	IAnimatedPropertyPercentageReady IPercentageSettings.With(Spec spec, int iterCount, AnimationDirection direction, AnimationFillMode fillMode)
		=> FluentlySetAll<IAnimatedPropertyPercentageReady>(spec, iterCount, direction, fillMode);
	IAnimatedPropertyLengthPercentageReady ILengthPercentageSettings.With(Spec spec, int iterCount, AnimationDirection direction, AnimationFillMode fillMode)
		=> FluentlySetAll<IAnimatedPropertyLengthPercentageReady>(spec, iterCount, direction, fillMode);
	private TThisSpecialized FluentlySetAll<TThisSpecialized>(Spec spec, int iterCount, AnimationDirection direction, AnimationFillMode fillMode)
		where TThisSpecialized : class
		=> DoFluently<TThisSpecialized>(() =>
		{
			_spec = spec;
			_iterationCount = iterCount;
			_direction = direction;
			_fillMode = fillMode;
		});



	private TThisSpecialized DoFluently<TThisSpecialized>(Action action)
		where TThisSpecialized : class
	{
		action();
		return (this as TThisSpecialized)!;
	}



	IAnimatedPropertyColorRegistration IAnimatedPropertyReadyToRegister<IAnimatedPropertyColorRegistration>.Create()
		=> Create();
	IAnimatedPropertyLengthRegistration IAnimatedPropertyReadyToRegister<IAnimatedPropertyLengthRegistration>.Create()
		=> Create();
	IAnimatedPropertyPercentageRegistration IAnimatedPropertyReadyToRegister<IAnimatedPropertyPercentageRegistration>.Create()
		=> Create();
	IAnimatedPropertyLengthPercentageRegistration IAnimatedPropertyReadyToRegister<IAnimatedPropertyLengthPercentageRegistration>.Create()
		=> Create();

	private AnimatedPropertyRegistration Create()
	{
		_creator.RegisterProperty(this);

		var marker = new AnimatedPropertyRegistration(
			unregister: () => _creator.UnregisterProperty(this),
			updateIsRunning: isRunning =>
			{
				_isRunning = isRunning;
				_creator.RefreshProperties();
			},
			nameWithoutPrefix: _name,
			_readStyleProperty,
			this);

		return marker;
	}


	private static void AssertPercentageBetween0and100Exclusive(CssPercentage percentage)
	{
		var percentAsNum = percentage.AsNumeric;

		if (percentAsNum > 0
			&& percentAsNum < 100)
		{
			return;
		}

		throw new Exception("Percentage value must be between 0 and 100 exclusive");
	}


	IAnimatedPropertyColor IAnimatedPropertyColor.Duplicate()
		=> FluentlyDuplicate<IAnimatedPropertyColor>();
	IAnimatedPropertyColorReady IAnimatedPropertyColorReady.Duplicate()
		=> FluentlyDuplicate<IAnimatedPropertyColorReady>();
	IAnimatedPropertyLength IAnimatedPropertyLength.Duplicate()
		=> FluentlyDuplicate<IAnimatedPropertyLength>();
	IAnimatedPropertyLengthReady IAnimatedPropertyLengthReady.Duplicate()
		=> FluentlyDuplicate<IAnimatedPropertyLengthReady>();
	IAnimatedPropertyPercentage IAnimatedPropertyPercentage.Duplicate()
		=> FluentlyDuplicate<IAnimatedPropertyPercentage>();
	IAnimatedPropertyPercentageReady IAnimatedPropertyPercentageReady.Duplicate()
		=> FluentlyDuplicate<IAnimatedPropertyPercentageReady>();
	IAnimatedPropertyLengthPercentage IAnimatedPropertyLengthPercentage.Duplicate()
		=> FluentlyDuplicate<IAnimatedPropertyLengthPercentage>();
	IAnimatedPropertyLengthPercentageReady IAnimatedPropertyLengthPercentageReady.Duplicate()
		=> FluentlyDuplicate<IAnimatedPropertyLengthPercentageReady>();
	private TThisSpecialized FluentlyDuplicate<TThisSpecialized>()
		where TThisSpecialized : class
		=> (Duplicate() as TThisSpecialized)!;
	private AnimatedProperty Duplicate()
		=> new(
			_creator,
			_readStyleProperty,
			_syntax,
			_initialValue,
			_finalValue)
		{
			_spec = _spec,
			_iterationCount = _iterationCount,
			_direction = _direction,
			_fillMode = _fillMode,
			_intermediateStates = _intermediateStates,
			_isRunning = _isRunning
		};

	
}
