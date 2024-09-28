using BlazorCssAnimations.Help;
using BlazorCssAnimations.JsInterop;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssAnimations;

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
