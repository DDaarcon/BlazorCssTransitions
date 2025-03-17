using static BlazorCssTransitions.Shared.ITimerService;

namespace BlazorCssTransitions.Shared;

internal interface ITimerService
{
    ITimerRegistration? StartNew(TimeSpan waitTime, Action actionToExecute, object? caller, ITimerRegistration? oldRegistration = null);

    interface ITimerRegistration
    {
        void Abort();
    }
}

internal class TimerService : ITimerService
{
    public ITimerRegistration? StartNew(TimeSpan waitTime, Action actionToExecute, object? caller, ITimerRegistration? oldRegistration = default)
    {
        if (waitTime <= TimeSpan.Zero)
        {
            actionToExecute();
            return null;
        }

        oldRegistration?.Abort();

        var timer = new TimerRegistration(waitTime);

        timer.Elapsed += (sender, args) =>
        {
            timer?.Dispose();

            actionToExecute();
        };

        timer.Start();

        return timer;
    }

    internal class TimerRegistration(TimeSpan interval) : System.Timers.Timer(interval), ITimerRegistration
    {
        public void Abort()
            => Dispose();
    }
}
