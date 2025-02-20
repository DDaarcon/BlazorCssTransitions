using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.CssStylesValidation;
using BlazorCssTransitions.UnitTests.Fakes;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorCssTransitions.UnitTests;

public class BCTTestContext : TestContext, IAsyncLifetime
{
    internal FakeTimerService TimerService => (FakeTimerService)Services.GetRequiredService<ITimerService>();

    public async Task InitializeAsync()
    {
        RegisterFakeServices();

        await OnTestSetup();
    }

    protected virtual Task OnTestSetup()
        => Task.CompletedTask;

    private void RegisterFakeServices()
    {
        Services.AddScoped<ICssStylesAppliedValidator, FakeCssStylesAppliedValidator>();
        Services.AddSingleton<ITimerService, FakeTimerService>();
    }


    public async Task DisposeAsync()
    {
        await OnTestTearDown();
    }

    protected virtual Task OnTestTearDown()
        => Task.CompletedTask;

    protected static readonly TimeSpan DebugTimeout = TimeSpan.FromHours(1);

    protected TaskCompletionSource CreateCompletionAwaiter(TimeSpan? customTimeout = null)
    {
        var completionAwaiter = new TaskCompletionSource();

        var cancellationSource = new CancellationTokenSource(customTimeout ?? TimeSpan.FromSeconds(10));

        cancellationSource.Token.Register(() => completionAwaiter.SetException(new Exception("Test timeout")));

        completionAwaiter.Task.ContinueWith(task => cancellationSource.Dispose());

        return completionAwaiter;
    }


    protected ImmediateRerenderAnalysisInContainerData<TComponent> RenderComponentInArtificalContainerForImmediateRerendersAnalysis<TComponent>(
        Action<ComponentParameterCollectionBuilder<TComponent>>? parameterBuilder = null,
        Action<IRenderedComponent<ArtificalContainer>, IRenderedComponent<TComponent>?>? onAfterRender = null,
        Action<IRenderedComponent<ArtificalContainer>, IRenderedComponent<TComponent>?>? onMarkupUpdated = null)
        where TComponent : IComponent
    {
        var container = RenderComponent<ArtificalContainer>();
        container.OnAfterRender += (sender, args) =>
        {
            if (sender is not IRenderedComponent<ArtificalContainer> container)
                throw new Exception("???");

            var component = container.FindComponent<TComponent>();
            onAfterRender?.Invoke(container, component);
        };
        
        container.OnMarkupUpdated += (sender, args) =>
        {
            if (sender is not IRenderedComponent<ArtificalContainer> container)
                throw new Exception("???");

            var component = container.FindComponent<TComponent>();
            onMarkupUpdated?.Invoke(container, component);
        };

        container.SetParametersAndRender(parameters => parameters
            .AddChildContent(parameterBuilder));

        var component = container.FindComponent<TComponent>();

        return new ImmediateRerenderAnalysisInContainerData<TComponent>(
            container,
            component);
    }


    protected record ImmediateRerenderAnalysisInContainerData<TComponent>(
        IRenderedComponent<ArtificalContainer> Container,
        IRenderedComponent<TComponent> Component)
        where TComponent : IComponent;
}
