using BlazorCssTransitions.Help;
using BlazorCssTransitions.JsInterop;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public static class RegistrationExtensions
{
    public static IServiceCollection AddBlazorCssAnimations(this IServiceCollection collection)
    {
        collection.AddScoped<JsInteropEntryPoint>();
        collection.AddScoped<ExternalRenderer>();

        collection.AddTransient<AnimatedListInternal.ItemsCollection>();

        return collection;
    }
}
