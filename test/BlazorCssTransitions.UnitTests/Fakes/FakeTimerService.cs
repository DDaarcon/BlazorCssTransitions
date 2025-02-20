using BlazorCssTransitions.Shared;

namespace BlazorCssTransitions.UnitTests.Fakes;

internal class FakeTimerService : ITimerService
{
    private readonly TimerService _realService = new();

    public void SetResultForAwaitingTimers(TimerAction actionForExisingTimers)
    {
        _timerControllingCompletionSource.SetResult(actionForExisingTimers);

        _timerControllingCompletionSource = new TaskCompletionSource<TimerAction>();
    }
    private TaskCompletionSource<TimerAction> _timerControllingCompletionSource = new();
    public bool UseRealTimers { get; set; }
    
    event Action<ITimerService.ITimerRegistration?>? OnTimerRegistered;

    public ITimerService.ITimerRegistration? StartNew(TimeSpan waitTime, Action actionToExecute, ITimerService.ITimerRegistration? oldRegistration = null)
    {
        if (UseRealTimers)
        {
            var timerReg = _realService.StartNew(waitTime, actionToExecute, oldRegistration);
            OnTimerRegistered?.Invoke(timerReg);
            return timerReg;
        }

        var completionSource = new TaskCompletionSource<TimerAction>();

        _timerControllingCompletionSource.Task.ContinueWith(result =>
        {
            completionSource.TrySetResult(result.Result);
        });

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
        });
        OnTimerRegistered?.Invoke(fakeTimerReg);

        return fakeTimerReg;
    }

    public class FakeTimerRegistration(Action abortAction) : ITimerService.ITimerRegistration
    {
        private readonly Action _abortAction = abortAction;

        public void Abort() => _abortAction();
    }

    public enum TimerAction
    {
        Abort,
        Act
    }
}
