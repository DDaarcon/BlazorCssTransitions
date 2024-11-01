using BlazorCssTransitions.AnimatedProperties;
using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.CssPropertyReading;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public partial class AnimatedPropertiesProvider : IDisposable
{
	[Inject]
	private AnimatedPropertiesCreatorImpl _creator { get; init; } = default!;
	[Inject]
	private JsCssPropertyReader _cssPropertyReader { get; init; } = default!;

	private readonly List<AnimatedProperty> _properties = [];

	private IDisposable? _registrationMarker;

	protected override void OnInitialized()
	{
		_registrationMarker = _creator.RegisterProvider(this);
	}

	public void Dispose()
	{
		_registrationMarker?.Dispose();
	}

	internal async Task<string?> ReadRootStyleProperty(string propertyName)
		=> await _cssPropertyReader.ReadRootProperty(propertyName);


	internal void AddProperty(AnimatedProperty property)
	{
		_properties.Add(property);
		UpdateStylesBlock();
	}

	internal void RemoveProperty(AnimatedProperty property)
	{
		_properties.Remove(property);
		UpdateStylesBlock();
	}

	internal void Refresh()
	{
		UpdateStylesBlock();
	}


	private MarkupString StylesBlock { get; set; }

	private void UpdateStylesBlock()
	{
		if (_properties.Count == 0)
		{
			StylesBlock = new MarkupString();
			return;
		}


		var builder = new StringBuilder();

		builder.AppendLine("""
			<style>
			""");

		AppendPropertiesDefinitionsToBuilder(builder);

		AppendRootDefinitionsToBuilder(builder);

		AppendKeyFramesDefinitionsToBuilder(builder);


		builder.AppendLine("</style>");

		StylesBlock = new MarkupString(builder.ToString());
		StateHasChanged();
	}

	private void AppendPropertiesDefinitionsToBuilder(StringBuilder builder)
	{
		foreach (var property in _properties)
		{
			builder.AppendLine($$"""
				@property --{{property.Name}} {
					syntax: "{{property.Syntax.ToCss()}}";
					inherits: true;
					initial-value: {{property.InitialValue}};
				}
				""");
		}
	}

	private void AppendRootDefinitionsToBuilder(StringBuilder builder)
	{
		builder.AppendLine(":root {");

		builder.AppendLine("animation: ");
		var composedAnimationDefinitions = String.Join(", ", _properties.Select(property => property.GetAnimationValue()));
		builder.Append(composedAnimationDefinitions);
		builder.AppendLine(";");
		builder.AppendLine("}");
	}

	private static string IterationCountCss(int iterCount)
		=> iterCount is AnimatedPropertiesCreator.InfiniteIterationCount
			? "infinite"
			: iterCount.ToString();

	private void AppendKeyFramesDefinitionsToBuilder(StringBuilder builder)
	{
		foreach (var property in _properties)
		{
			if (property.IntermediateStates?.Any() ?? false)
			{
				builder.AppendLine($"@keyframes {property.AnimationName} {{");
				builder.AppendLine($"0% {{ --{property.Name}: {property.InitialValue}; }}");

				foreach (var intermediateState in property.IntermediateStates.OrderBy(x => x.Key))
				{
					builder.AppendLine($"{intermediateState.Key} {{ --{property.Name}: {intermediateState.Value}; }}");
				}

                builder.AppendLine($"100% {{ --{property.Name}: {property.FinalValue}; }}");
				builder.AppendLine("}");

            }
            else
			{
				builder.AppendLine($$"""
					@keyframes {{property.AnimationName}} {
						from {
							--{{property.Name}}: {{property.InitialValue}};
						}
						to {
							--{{property.Name}}: {{property.FinalValue}};
						}
					}
					""");
			}
		}
	}
}
