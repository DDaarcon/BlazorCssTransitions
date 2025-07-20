using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorCssTransitions.Shared.SizeMeasurement;

internal class JsSizeObserver(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask
        = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazorCssTransitions/size-observer.js").AsTask());

    /// <summary>
    /// Returns instance of listener, used for registration. Dispose that element when callback will no longer be needed;
    /// </summary>
    /// <returns></returns>
    public async Task<IAsyncDisposable> ListenForElementSizeChanges(ElementReference element, Func<DOMScrollRect, Task> onTrigger)
    {
        var module = await moduleTask.Value;

        var jsCallback = new JsCallback<DOMScrollRect>()
        {
            OnTrigger = onTrigger
        };
        var callbackListenerJsInstance = DotNetObjectReference.Create(jsCallback);
        jsCallback.DotNetReference = callbackListenerJsInstance;

        var jsResizeObserver = await module.InvokeAsync<IJSObjectReference>("listenForSizeChanges", element, callbackListenerJsInstance);


        jsCallback.OnDispose = async () =>
        {
            await module.InvokeVoidAsync("stopListeningFoSizeChanges", jsResizeObserver);
            await jsResizeObserver.DisposeAsync();
        };

        return jsCallback;
    }

    internal class JsCallback<TCallbackParam>() : IAsyncDisposable
    {
        public DotNetObjectReference<JsCallback<TCallbackParam>> DotNetReference { get; set; } = default!;
        public Func<TCallbackParam, Task> OnTrigger { get; init; } = default!;
        public Func<Task> OnDispose { get; set; } = default!;

        public async ValueTask DisposeAsync()
        {
            await OnDispose();
            DotNetReference.Dispose();
        }

        [JSInvokable]
        public async Task Invoke(TCallbackParam param)
        {
            await OnTrigger(param);
        }


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
