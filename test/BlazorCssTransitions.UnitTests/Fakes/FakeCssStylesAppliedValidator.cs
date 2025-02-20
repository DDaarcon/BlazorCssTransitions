using BlazorCssTransitions.Shared.CssStylesValidation;
using Microsoft.AspNetCore.Components;

namespace BlazorCssTransitions.UnitTests.Fakes;

internal class FakeCssStylesAppliedValidator : ICssStylesAppliedValidator
{
    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;

    public Task EnsureStylesWereApplied(ElementReference element)
    {
        return Task.CompletedTask;
    }
}
