using BlazorCssTransitions.AnimatedProperties;
using BlazorCssTransitions.Shared.CssPropertyReading;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public class AnimatedPropertyRegistration 
	: IDisposable,
		IAnimatedPropertyColorRegistration, IAnimatedPropertyLengthRegistration,
		IAnimatedPropertyPercentageRegistration, IAnimatedPropertyLengthPercentageRegistration
{
	/// <summary>
	/// Name of the property
	/// </summary>
	public string FullName => _fullName;
	/// <summary>
	/// Name of the property wrapped in "var(...)". Ready to be applied to css.
	/// </summary>
	public string InvokableName => $"var({_fullName})";

	public void Pause()
		=> _updateIsRunnning(false);

	public void Resume()
		=> _updateIsRunnning(true);



	private readonly Action _unregister;
	private readonly Action<bool> _updateIsRunnning;
	private readonly string _fullName;
	private readonly Func<string, Task<(AnimatedPropertiesCreatorImpl.ReadStylePropertyResult, string?)>> _readStyleProperty;

	internal AnimatedPropertyRegistration(
		Action unregister,
		Action<bool> updateIsRunning,
		string nameWithoutPrefix,
		Func<string, Task<(AnimatedPropertiesCreatorImpl.ReadStylePropertyResult, string?)>> readStyleProperty)
	{
		_unregister = unregister;
		_updateIsRunnning = updateIsRunning;
		_fullName = "--" + nameWithoutPrefix;
		_readStyleProperty = readStyleProperty;
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
