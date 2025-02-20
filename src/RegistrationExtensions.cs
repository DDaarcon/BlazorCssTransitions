using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.CssPropertyReading;
using BlazorCssTransitions.Shared.CssStylesValidation;
using BlazorCssTransitions.Shared.JsInterop;
using BlazorCssTransitions.Shared.SizeMeasurement;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BlazorCssTransitions.UnitTests")]

namespace BlazorCssTransitions;

public static class RegistrationExtensions
{
    public static IServiceCollection AddBlazorCssAnimations(this IServiceCollection collection)
    {
        collection.AddScoped<JsSizeMeter>();
        collection.AddScoped<JsSizeObserver>();
        collection.AddScoped<JsCssPropertyReader>();
        collection.AddScoped<ICssStylesAppliedValidator, JsCssStylesAppliedValidator>();
        collection.AddSingleton<ITimerService, TimerService>();

        collection.AddScoped<ExternalRenderer>();

        collection.AddTransient<AnimatedListInternal.ItemsCollection>();

        collection.AddSingleton<AnimatedPropertiesCreatorImpl>();
        collection.AddSingleton<AnimatedPropertiesCreator>(services => services.GetRequiredService<AnimatedPropertiesCreatorImpl>());

        return collection;
    }
}
