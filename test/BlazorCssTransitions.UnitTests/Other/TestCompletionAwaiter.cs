using Bunit;

namespace BlazorCssTransitions.UnitTests.Other;

internal class TestCompletionAwaiter
{

    /// <summary>
    /// To be used in <see cref="CreateCompletionAwaiter(TimeSpan?)"/> when debugging test
    /// </summary>
    protected static readonly TimeSpan DebugTimeout = TimeSpan.FromHours(1);
    public static TestCompletionAwaiter CreateDebug(TimeSpan? ignoredParameter = null, Action? onTimeout = null)
        => Create(DebugTimeout, onTimeout);

    public static TestCompletionAwaiter Create(TimeSpan? customTimeout = null, Action? onTimeout = null)
    {
        var completionAwaiter = new TaskCompletionSource();

        var cancellationSource = new CancellationTokenSource(customTimeout ?? TimeSpan.FromSeconds(5));

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



    // instance methods

    private TestCompletionAwaiter(TaskCompletionSource source)
    {
        _source = source;
    }
    private readonly TaskCompletionSource _source;

    public void MarkAsFinished()
    {
        if (_source.Task.IsFaulted)
            throw _source.Task.Exception;
        if (_source.Task.IsCompleted)
            return;
        _source.SetResult();
    }

    public async Task ActAndMarkAsFinished(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            _source.SetException(ex);
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
            _source.SetException(ex);
            return;
        }
        MarkAsFinished();
    }

    public async Task WaitForFinish()
        => await _source.Task;

    public TestCompletionAwaiter ScheduleCallbackAfterNextMarkupUpdateOfElement(
        IRenderedFragment element,
        Action actionCausingUpdate,
        Action onMarkupChange)
    {
        element.OnMarkupUpdated += OnMarkupUpdate;
        void OnMarkupUpdate(object? sender, object args)
        {
            onMarkupChange();
            element.OnMarkupUpdated -= OnMarkupUpdate;
            MarkAsFinished();
        }
        actionCausingUpdate();

        return this;
    }
}
