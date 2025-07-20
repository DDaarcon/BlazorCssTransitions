using BlazorCssTransitions.Shared.SizeMeasurement;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BlazorCssTransitions.Shared.SizeMeasurement.JsSizeObserver;

namespace BlazorCssTransitions.Shared.CssPropertyReading;

internal class JsCssPropertyReader(IJSRuntime jsRuntime) : IAsyncDisposable
{
	private readonly Lazy<Task<IJSObjectReference>> moduleTask
		= new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
			"import", "./_content/BlazorCssTransitions/css-properties-reader.js").AsTask());


	/// Returns instance of listener, used for registration. Dispose that element when callback will no longer be needed;
	/// </summary>
	/// <returns></returns>
	public async Task<string?> ReadRootProperty(string propertyName)
	{
		var module = await moduleTask.Value;

		return await module.InvokeAsync<string?>("readRootProperty", propertyName);
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
