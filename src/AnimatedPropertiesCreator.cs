using BlazorCssTransitions.AnimatedProperties;
using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.CssPropertyReading;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public interface AnimatedPropertiesCreator
{
	/// <summary>
	/// Intializes creation of color css property. 
	/// </summary>
	IAnimatedPropertyColor NewColorProperty(Color initialColor, Color finalColor);
	IAnimatedPropertyLengthPercentage NewLengthPercentageProperty(CssLengthPercentage initialValue, CssLengthPercentage finalValue);
	IAnimatedPropertyLength NewLengthProperty(CssLength initialValue, CssLength finalValue);
	IAnimatedPropertyPercentage NewPercentageProperty(CssPercentage initialValue, CssPercentage finalValue);


	public const int InfiniteIterationCount = -1;
}

internal class AnimatedPropertiesCreatorImpl(
	JsCssPropertyReader cssPropertyReader) : AnimatedPropertiesCreator
{
	private readonly JsCssPropertyReader _cssPropertyReader = cssPropertyReader;

	public IAnimatedPropertyColor NewColorProperty(Color initialColor, Color finalColor)
		=> new AnimatedProperty(
			this, ReadRootStyleProperty,
			syntax: PropertySyntax.Color,
			initialValue: initialColor.ToHex(),
			finalValue: finalColor.ToHex());
	public IAnimatedPropertyLength NewLengthProperty(CssLength initialValue, CssLength finalValue)
		=> new AnimatedProperty(
			this, ReadRootStyleProperty,
			syntax: PropertySyntax.Length,
			initialValue,
			finalValue);
	public IAnimatedPropertyPercentage NewPercentageProperty(CssPercentage initialValue, CssPercentage finalValue)
		=> new AnimatedProperty(
			this, ReadRootStyleProperty,
			syntax: PropertySyntax.Percentage,
			initialValue,
			finalValue);
	public IAnimatedPropertyLengthPercentage NewLengthPercentageProperty(CssLengthPercentage initialValue, CssLengthPercentage finalValue)
		=> new AnimatedProperty(
			this, ReadRootStyleProperty,
			syntax: PropertySyntax.Percentage,
			initialValue,
			finalValue);


	private AnimatedPropertiesProvider? _provider;
	internal IDisposable? RegisterProvider(AnimatedPropertiesProvider provider)
	{
		if (_provider is not null)
			return null;

		_provider = provider;

		return new ProviderRegistrationMarker(() => _provider = null);
	}
	private class ProviderRegistrationMarker(Action unregister) : IDisposable
	{
		private readonly Action _unregister = unregister;

		public void Dispose()
		{
			_unregister();
		}
	}


	internal void RegisterProperty(AnimatedProperty property)
	{
		_provider?.AddProperty(property);
	}

	internal void UnregisterProperty(AnimatedProperty property)
	{
		_provider?.RemoveProperty(property);
	}

	internal void RefreshProperties()
	{
		_provider?.Refresh();
	}

	internal async Task<(ReadStylePropertyResult result, string? value)> ReadRootStyleProperty(string propertyName)
	{
		if (_provider is null)
			return (ReadStylePropertyResult.NotYetInitialized, null);

		return (
			ReadStylePropertyResult.Success,
			await _provider.ReadRootStyleProperty(propertyName)
		);
	}

	internal enum ReadStylePropertyResult
	{
		Success,
		NotYetInitialized
	}
}
