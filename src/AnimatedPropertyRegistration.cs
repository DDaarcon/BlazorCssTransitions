using BlazorCssTransitions.AnimatedProperties;
using BlazorCssTransitions.Shared.CssPropertyReading;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

internal class AnimatedPropertyRegistration 
	: IAnimatedPropertyColorRegistration, IAnimatedPropertyLengthRegistration,
		IAnimatedPropertyPercentageRegistration, IAnimatedPropertyLengthPercentageRegistration
{
	public string FullName => _fullName;
	public string InvokableName => $"var({_fullName})";

    public void Pause()
		=> _updateIsRunnning(false);

	public void Resume()
		=> _updateIsRunnning(true);

    IAnimatedPropertyColorFields IAnimatedPropertyColorRegistration.State => _animatedProperty;
    IAnimatedPropertyLengthFields IAnimatedPropertyLengthRegistration.State => _animatedProperty;
    IAnimatedPropertyPercentageFields IAnimatedPropertyPercentageRegistration.State => _animatedProperty;
    IAnimatedPropertyLengthPercentageFields IAnimatedPropertyLengthPercentageRegistration.State => _animatedProperty;


    private readonly Action _unregister;
	private readonly Action<bool> _updateIsRunnning;
	private readonly string _fullName;
	private readonly Func<string, Task<(AnimatedPropertiesCreatorImpl.ReadStylePropertyResult, string?)>> _readStyleProperty;
	private readonly AnimatedProperty _animatedProperty;

    internal AnimatedPropertyRegistration(
        Action unregister,
        Action<bool> updateIsRunning,
        string nameWithoutPrefix,
        Func<string, Task<(AnimatedPropertiesCreatorImpl.ReadStylePropertyResult, string?)>> readStyleProperty,
        AnimatedProperty animatedProperty)
    {
        _unregister = unregister;
        _updateIsRunnning = updateIsRunning;
        _fullName = "--" + nameWithoutPrefix;
        _readStyleProperty = readStyleProperty;
        _animatedProperty = animatedProperty;
    }

    public void Dispose()
	{
		_unregister();
	}

	async Task<Color?> IAnimatedPropertyColorRegistration.GetCurrentValue()
	{
		var valueString = await ReadPropertyValueString();

		if (valueString is null)
			return null;

		return CssColorParser.AttemptParse(valueString);
	}
	async Task<CssLength> IAnimatedPropertyLengthRegistration.GetCurrentValue()
	{
		var valueString = await ReadPropertyValueString();

		if (valueString is null)
			return CssLength.Unassigned();

		return valueString;
	}
	async Task<CssPercentage> IAnimatedPropertyPercentageRegistration.GetCurrentValue()
	{
		var valueString = await ReadPropertyValueString();

		if (valueString is null)
			return CssPercentage.Unassigned();

		return valueString;
	}
	async Task<CssLengthPercentage> IAnimatedPropertyLengthPercentageRegistration.GetCurrentValue()
	{
		var valueString = await ReadPropertyValueString();

		if (valueString is null)
			return CssLengthPercentage.Unassigned();

		return valueString;
	}


	private async Task<string?> ReadPropertyValueString()
	{
		var (result, value) = await _readStyleProperty(_fullName);
		return value;
	}
}
