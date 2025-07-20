using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorCssTransitions.Shared.SizeMeasurement;

internal class JsSizeMeter(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask
        = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazorCssTransitions/size-meter.js").AsTask());

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
