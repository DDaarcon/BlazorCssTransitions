using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.Shared;

internal class TimerHelper
{
    public static System.Timers.Timer? StartNewOneTimeTimer(TimeSpan waitTime, Action actionToExecute)
    {
        if (waitTime <= TimeSpan.Zero)
        {
            actionToExecute();
            return null;
        }

        var timer = new System.Timers.Timer(waitTime);

        timer.Elapsed += (sender, args) =>
        {
            timer?.Dispose();

            actionToExecute();
        };

        timer.Start();

        return timer;
    }
}
