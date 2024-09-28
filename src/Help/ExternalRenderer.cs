using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.HtmlRendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace BlazorCssTransitions.Help;

/// <remarks>
/// Intended as singleton/scoped
/// </remarks>
internal class ExternalRenderer(ILoggerFactory loggerFactory)
{
    private readonly ILoggerFactory _loggerFactory = loggerFactory;

    private readonly Lazy<IServiceProvider> _lazyServiceProvider = new(() => new ServiceCollection().BuildServiceProvider());


    public async Task Render(RenderFragment fragment)
    {
        await using var htmlRenderer = new HtmlRenderer(_lazyServiceProvider.Value, _loggerFactory);

        await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            _ = await RenderElementOnRenderer(htmlRenderer, fragment);
        });
    }

    private Task<HtmlRootComponent> RenderElementOnRenderer(HtmlRenderer renderer, RenderFragment fragment)
    {
        var dictionary = new Dictionary<string, object?>
        {
          { "ChildContent", fragment }
        };
        var parameters = ParameterView.FromDictionary(dictionary);
        return renderer.RenderComponentAsync<FragmentContainer>(parameters);
    }

    private sealed class FragmentContainer : IComponent
    {
        private RenderHandle renderHandle;

        public void Attach(RenderHandle renderHandle) => this.renderHandle = renderHandle;

        public Task SetParametersAsync(ParameterView parameters)
        {
            if (parameters.TryGetValue<RenderFragment>("ChildContent", out var childContent))
            {
                renderHandle.Render(childContent);
            }
            return Task.CompletedTask;
        }
    }
}
