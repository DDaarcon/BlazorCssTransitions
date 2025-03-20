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

    /// <summary>
    /// To be used in <see cref="CreateCompletionAwaiter(TimeSpan?)"/> when debugging test
    /// </summary>
    protected static readonly TimeSpan DebugTimeout = TimeSpan.FromHours(1);


    protected TestCompletionAwaiter CreateCompletionAwaiter(TimeSpan? customTimeout = null, Action? onTimeout = null)
    {
        var completionAwaiter = new TaskCompletionSource();

        // normally tests should not take more than 10 sec
        var cancellationSource = new CancellationTokenSource(customTimeout ?? TimeSpan.FromSeconds(10));
        
        cancellationSource.Token.Register(() =>
        {
            if (onTimeout is not null)
            {
                try
                {
                    onTimeout();
                    completionAwaiter.SetCanceled();
                }
                catch (Exception ex)
                {
                    completionAwaiter.SetException(ex);
                }
            }
            else
            {
                completionAwaiter.SetException(new Exception("Test timeout"));
            }
        });

        completionAwaiter.Task.ContinueWith(task => cancellationSource.Dispose());

        return new TestCompletionAwaiter(completionAwaiter);
    }

    protected class TestCompletionAwaiter(TaskCompletionSource source)
    {
        public void MarkAsFinished()
        {
            if (source.Task.IsFaulted)
                throw source.Task.Exception;
            if (source.Task.IsCompleted)
                return;
            source.SetResult();
        }

        public async Task ActAndMarkAsFinished(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                source.SetException(ex);
                return;
            }
            MarkAsFinished();
        }

        public void ActAndMarkAsFinished(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                source.SetException(ex);
                return;
            }
            MarkAsFinished();
        }

        public async Task WaitForFinish()
            => await source.Task;
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
