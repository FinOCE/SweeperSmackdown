using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Functions.Activities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public class TimerOrchestratorFunctionProps
{
    public int? Duration { get; }

    public string? RaiseEventTo { get; }

    public TimerOrchestratorFunctionProps(int? duration = null, string? raiseEventTo = null)
    {
        Duration = duration;
        RaiseEventTo = raiseEventTo;
    }
}

public static class TimerOrchestratorFunction
{
    [FunctionName(nameof(TimerOrchestratorFunction))]
    public static async Task Run([OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var props = ctx.GetInput<TimerOrchestratorFunctionProps>();
        var tasks = new List<Task>();

        ctx.SetCustomStatus(ETimerStatus.NotStarted);

        // Wait for setup countdown to be initiated
        var expiration = props.Duration == null
            ? await ctx.WaitForExternalEvent<DateTime>(DurableEvents.START_TIMER)
            : ctx.CurrentUtcDateTime.AddSeconds(props.Duration.Value);

        ctx.SetCustomStatus(ETimerStatus.Countdown);

        // Start countdown
        using var timeoutCts = new CancellationTokenSource();
        var timeoutTask = ctx.CreateTimer(expiration, timeoutCts.Token);

        if (props.Duration != 0)
            tasks.Add(timeoutTask);

        // Wait for external events or timeout expiring
        var resetTask = ctx.WaitForExternalEvent(DurableEvents.RESET_TIMER);
        tasks.Add(resetTask);
        var skipTask = ctx.WaitForExternalEvent(DurableEvents.SKIP_TIMER);
        tasks.Add(skipTask);
        var cancelTask = ctx.WaitForExternalEvent(DurableEvents.CANCEL_TIMER);
        tasks.Add(cancelTask);

        var winner = await Task.WhenAny(tasks);

        if (!timeoutTask.IsCompleted)
            timeoutCts.Cancel();
        else
            ctx.SetCustomStatus(ETimerStatus.Completed);

        if (winner == resetTask)
            ctx.ContinueAsNew(new TimerOrchestratorFunctionProps());
        else if (props.RaiseEventTo != null)
            await ctx.CallActivityAsync(
                nameof(TimerCompletedActivityFunction),
                new TimerCompletedActivityFunctionProps(props.RaiseEventTo));
    }
}

public enum ETimerStatus
{
    NotStarted,
    Countdown,
    Completed
}
