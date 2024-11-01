using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Specifications;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedProperties;

internal class AnimatedProperty 
	: IAnimatedPropertyColor, IAnimatedPropertyColorReady,
		IAnimatedPropertyLength, IAnimatedPropertyLengthReady,
		IAnimatedPropertyPercentage, IAnimatedPropertyPercentageReady,
		IAnimatedPropertyLengthPercentage, IAnimatedPropertyLengthPercentageReady
{
	private readonly AnimatedPropertiesCreator _creator;

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


	internal string Name => _name;
	internal string AnimationName => _name + "-anim";
	internal PropertySyntax Syntax => _syntax;
	internal string InitialValue => _initialValue;
	internal string FinalValue => _finalValue;
	private Spec Spec => _spec ?? throw new ArgumentNullException("Specification (spec) must be provided to construct an animated property");

	internal string GetAnimationValue()
		=> $"{Spec.GetAnimationValue()} {IterationCountCss(_iterationCount)} {_direction.ToCss()} {_fillMode.ToCss()} running {AnimationName}";


	private static string IterationCountCss(int iterCount)
		=> iterCount is AnimatedPropertiesCreator.InfiniteIterationCount
			? "infinite"
			: iterCount.ToString();


	public AnimatedProperty(
		AnimatedPropertiesCreator creator,
		PropertySyntax syntax,
		string initialValue,
		string finalValue)
	{
		_creator = creator;
		_syntax = syntax;
		_initialValue = initialValue;
		_finalValue = finalValue;
	}


	IAnimatedPropertyColorReady IAnimatedPropertySettings<IAnimatedPropertyColor, IAnimatedPropertyColorReady>.WithSpec(Spec spec)
		=> FluentlySetSpec<IAnimatedPropertyColorReady>(spec);
	IAnimatedPropertyLengthReady IAnimatedPropertySettings<IAnimatedPropertyLength, IAnimatedPropertyLengthReady>.WithSpec(Spec spec)
		=> FluentlySetSpec<IAnimatedPropertyLengthReady>(spec);
	IAnimatedPropertyPercentageReady IAnimatedPropertySettings<IAnimatedPropertyPercentage, IAnimatedPropertyPercentageReady>.WithSpec(Spec spec)
		=> FluentlySetSpec<IAnimatedPropertyPercentageReady>(spec);
	IAnimatedPropertyLengthPercentageReady IAnimatedPropertySettings<IAnimatedPropertyLengthPercentage, IAnimatedPropertyLengthPercentageReady>.WithSpec(Spec spec)
		=> FluentlySetSpec<IAnimatedPropertyLengthPercentageReady>(spec);
	private TThisSpecialized FluentlySetSpec<TThisSpecialized>(Spec spec)
		where TThisSpecialized : class
		=> DoFluently<TThisSpecialized>(() => _spec = spec);


	IAnimatedPropertyColor IAnimatedPropertySettings<IAnimatedPropertyColor, IAnimatedPropertyColorReady>.WithIterationCount(int count)
		=> FluentlySetIterationCount<IAnimatedPropertyColor>(count);
	IAnimatedPropertyLength IAnimatedPropertySettings<IAnimatedPropertyLength, IAnimatedPropertyLengthReady>.WithIterationCount(int count)
		=> FluentlySetIterationCount<IAnimatedPropertyLength>(count);
	IAnimatedPropertyPercentage IAnimatedPropertySettings<IAnimatedPropertyPercentage, IAnimatedPropertyPercentageReady>.WithIterationCount(int count)
		=> FluentlySetIterationCount<IAnimatedPropertyPercentage>(count);
	IAnimatedPropertyLengthPercentage IAnimatedPropertySettings<IAnimatedPropertyLengthPercentage, IAnimatedPropertyLengthPercentageReady>.WithIterationCount(int count)
		=> FluentlySetIterationCount<IAnimatedPropertyLengthPercentage>(count);
	private TThisSpecialized FluentlySetIterationCount<TThisSpecialized>(int count)
		where TThisSpecialized : class
		=> DoFluently<TThisSpecialized>(() => _iterationCount = count);


	IAnimatedPropertyColor IAnimatedPropertySettings<IAnimatedPropertyColor, IAnimatedPropertyColorReady>.WithDirection(AnimationDirection direction)
		=> FluentlySetDirection<IAnimatedPropertyColor>(direction);
	IAnimatedPropertyLength IAnimatedPropertySettings<IAnimatedPropertyLength, IAnimatedPropertyLengthReady>.WithDirection(AnimationDirection direction)
		=> FluentlySetDirection<IAnimatedPropertyLength>(direction);
	IAnimatedPropertyPercentage IAnimatedPropertySettings<IAnimatedPropertyPercentage, IAnimatedPropertyPercentageReady>.WithDirection(AnimationDirection direction)
		=> FluentlySetDirection<IAnimatedPropertyPercentage>(direction);
	IAnimatedPropertyLengthPercentage IAnimatedPropertySettings<IAnimatedPropertyLengthPercentage, IAnimatedPropertyLengthPercentageReady>.WithDirection(AnimationDirection direction)
		=> FluentlySetDirection<IAnimatedPropertyLengthPercentage>(direction);
	private TThisSpecialized FluentlySetDirection<TThisSpecialized>(AnimationDirection direction)
		where TThisSpecialized : class
		=> DoFluently<TThisSpecialized>(() => _direction = direction);


	IAnimatedPropertyColor IAnimatedPropertySettings<IAnimatedPropertyColor, IAnimatedPropertyColorReady>.WithFillMode(AnimationFillMode fillMode)
		=> FluentlySetFillMode<IAnimatedPropertyColor>(fillMode);
	IAnimatedPropertyLength IAnimatedPropertySettings<IAnimatedPropertyLength, IAnimatedPropertyLengthReady>.WithFillMode(AnimationFillMode fillMode)
		=> FluentlySetFillMode<IAnimatedPropertyLength>(fillMode);
	IAnimatedPropertyPercentage IAnimatedPropertySettings<IAnimatedPropertyPercentage, IAnimatedPropertyPercentageReady>.WithFillMode(AnimationFillMode fillMode)
		=> FluentlySetFillMode<IAnimatedPropertyPercentage>(fillMode);
	IAnimatedPropertyLengthPercentage IAnimatedPropertySettings<IAnimatedPropertyLengthPercentage, IAnimatedPropertyLengthPercentageReady>.WithFillMode(AnimationFillMode fillMode)
		=> FluentlySetFillMode<IAnimatedPropertyLengthPercentage>(fillMode);
	private TThisSpecialized FluentlySetFillMode<TThisSpecialized>(AnimationFillMode fillMode)
		where TThisSpecialized : class
		=> DoFluently<TThisSpecialized>(() => _fillMode = fillMode);


	IAnimatedPropertyColor IAnimatedPropertyColor.SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, Color>> steps)
		=> FluentlySetIntermediateSteps<IAnimatedPropertyColor, Color>(steps, value => value.ToHex());
	IAnimatedPropertyLength IAnimatedPropertyLength.SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, CssLength>> steps)
		=> FluentlySetIntermediateSteps<IAnimatedPropertyLength, CssLength>(steps, value => value);
	IAnimatedPropertyPercentage IAnimatedPropertyPercentage.SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, CssPercentage>> steps)
		=> FluentlySetIntermediateSteps<IAnimatedPropertyPercentage, CssPercentage>(steps, value => value);
	IAnimatedPropertyLengthPercentage IAnimatedPropertyLengthPercentage.SetIntermediateSteps(IEnumerable<KeyValuePair<CssPercentage, CssLengthPercentage>> steps)
		=> FluentlySetIntermediateSteps<IAnimatedPropertyLengthPercentage, CssLengthPercentage>(steps, value => value);
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


	IAnimatedPropertyColorReady IAnimatedPropertySettings<IAnimatedPropertyColor, IAnimatedPropertyColorReady>.With(Spec spec, int iterCount, AnimationDirection direction, AnimationFillMode fillMode)
		=> FluentlySetAll<IAnimatedPropertyColorReady>(spec, iterCount, direction, fillMode);
	IAnimatedPropertyLengthReady IAnimatedPropertySettings<IAnimatedPropertyLength, IAnimatedPropertyLengthReady>.With(Spec spec, int iterCount, AnimationDirection direction, AnimationFillMode fillMode)
		=> FluentlySetAll<IAnimatedPropertyLengthReady>(spec, iterCount, direction, fillMode);
	IAnimatedPropertyPercentageReady IAnimatedPropertySettings<IAnimatedPropertyPercentage, IAnimatedPropertyPercentageReady>.With(Spec spec, int iterCount, AnimationDirection direction, AnimationFillMode fillMode)
		=> FluentlySetAll<IAnimatedPropertyPercentageReady>(spec, iterCount, direction, fillMode);
	IAnimatedPropertyLengthPercentageReady IAnimatedPropertySettings<IAnimatedPropertyLengthPercentage, IAnimatedPropertyLengthPercentageReady>.With(Spec spec, int iterCount, AnimationDirection direction, AnimationFillMode fillMode)
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


	/// <summary>
	/// Registers property in provider (renders it on the page).
	/// </summary>
	/// <returns>
	/// A disposable property registration.
	/// </returns>
	public AnimatedPropertyRegistration Create()
	{
		_creator.RegisterProperty(this);

		var marker = new AnimatedPropertyRegistration(
			unregister: () => _creator.UnregisterProperty(this),
			updateIsRunning: isRunning =>
			{
				_isRunning = isRunning;
				_creator.RefreshProperties();
			},
			nameWithoutPrefix: _name);

		return marker;
	}


	private static void AssertPercentageBetween0and100Exclusive(CssPercentage percentage)
	{
		var percentAsNum = percentage.ToNumeric();

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
