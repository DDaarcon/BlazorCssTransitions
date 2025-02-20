using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorCssTransitions.Shared.CssStylesValidation;

internal interface ICssStylesAppliedValidator : IAsyncDisposable
{
    Task EnsureStylesWereApplied(ElementReference element);
}

internal class JsCssStylesAppliedValidator(IJSRuntime jsRuntime) : ICssStylesAppliedValidator
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask
        = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazorCssTransitions/css-styles-applied-validator.js").AsTask());

    public async Task EnsureStylesWereApplied(ElementReference element)
    {
        var module = await moduleTask.Value;

        await module.InvokeVoidAsync("ensureStylesWereApplied", element);
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
