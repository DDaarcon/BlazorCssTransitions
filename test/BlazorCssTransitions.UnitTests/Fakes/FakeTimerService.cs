using BlazorCssTransitions.Shared;

namespace BlazorCssTransitions.UnitTests.Fakes;

internal class FakeTimerService : ITimerService
{
    private readonly TimerService _realService = new();

    public void SetResultForAllAwaitingTimers(TimerAction action)
    {
        using (_lock.EnterScope())
        {
            foreach (var item in _completionSources.SelectMany(x => x.Value))
            {
                item.SetResult(action);
            }

            _completionSources.Clear();
        }
    }

    public void SetResultForAwaitingTimers(object? caller, TimerAction action)
    {
        using (_lock.EnterScope())
        {
            if (_completionSources.TryGetValue(caller?.ToString() ?? "", out var list))
            {
                foreach (var item in list)
                {
                    item.SetResult(action);
                }
                _completionSources.Remove(caller?.ToString() ?? "");
            }
        }
    }

    private Lock _lock = new();

    public bool UseRealTimers { get; set; }
    
    event Action<ITimerService.ITimerRegistration?>? OnTimerRegistered;
    event Action<FakeTimerRegistration?>? OnFakeTimerRegistered;

    private Dictionary<string, List<TaskCompletionSource<TimerAction>>> _completionSources = new();

    public IEnumerable<string> CallerKeys => _completionSources.Keys;

    public ITimerService.ITimerRegistration? StartNew(TimeSpan waitTime, Action actionToExecute, object? caller, ITimerService.ITimerRegistration? oldRegistration = null)
    {

        if (UseRealTimers)
        {
            var timerReg = _realService.StartNew(waitTime, actionToExecute, caller, oldRegistration);
            OnTimerRegistered?.Invoke(timerReg);
            return timerReg;
        }

        var completionSource = new TaskCompletionSource<TimerAction>();

        using (_lock.EnterScope())
        {
            if (!_completionSources.TryGetValue(caller?.ToString() ?? "", out var list))
            {
                list = [];
            }
            list.Add(completionSource);

            _completionSources[caller?.ToString() ?? ""] = list;
        }

        Task.Run(async () =>
        {
            switch (await completionSource.Task)
            {
                case TimerAction.Abort:
                    return;
                case TimerAction.Act:
                    actionToExecute();
                    break;
            }
        });

        var fakeTimerReg = new FakeTimerRegistration(() =>
        {
            completionSource.TrySetResult(TimerAction.Abort);
        }, caller);
        OnTimerRegistered?.Invoke(fakeTimerReg);
        OnFakeTimerRegistered?.Invoke(fakeTimerReg);

        return fakeTimerReg;
    }

    public class FakeTimerRegistration(Action abortAction, object? caller) : ITimerService.ITimerRegistration
    {
        private readonly Action _abortAction = abortAction;
        public object? Caller { get; } = caller;
        public void Abort() => _abortAction();
    }

    public enum TimerAction
    {
        Abort,
        Act
    }
}
