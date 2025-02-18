﻿using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.CssPropertyReading;
using BlazorCssTransitions.Shared.JsInterop;
using BlazorCssTransitions.Shared.SizeMeasurement;
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
        collection.AddScoped<JsSizeMeter>();
        collection.AddScoped<JsSizeObserver>();
        collection.AddScoped<JsCssPropertyReader>();

        collection.AddScoped<ExternalRenderer>();

        collection.AddTransient<AnimatedListInternal.ItemsCollection>();

        collection.AddSingleton<AnimatedPropertiesCreatorImpl>();
        collection.AddSingleton<AnimatedPropertiesCreator>(services => services.GetRequiredService<AnimatedPropertiesCreatorImpl>());

        return collection;
    }
}
