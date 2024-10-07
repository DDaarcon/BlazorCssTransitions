using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.Shared.JsInterop;

// TODO for now everything will be in one class, if there will be more js functionality then might consider splitting
internal class JsSizeMeter(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask
        = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazorCssTransitions/scripts.js").AsTask());

    internal async ValueTask<DOMRect> MeasureElement(ElementReference element)
    {
        var module = await this.moduleTask.Value;
        return await module.InvokeAsync<DOMRect>("measureElement", element);
    }

    internal async ValueTask<DOMScrollRect> MeasureElementScroll(ElementReference element)
    {
        var module = await this.moduleTask.Value;
        return await module.InvokeAsync<DOMScrollRect>("measureElementScroll", element);
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await this.moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
