using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public class TimerOrchestratorFunctionProps
{
    public int Duration { get; }

    public bool StartInstantly { get; }

    public TimerOrchestratorFunctionProps(int duration, bool startInstantly)
    {
        Duration = duration;
        StartInstantly = startInstantly;
    }
}

public static class TimerOrchestratorFunction
{
    [FunctionName(nameof(TimerOrchestratorFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var props = ctx.GetInput<TimerOrchestratorFunctionProps>();
        var tasks = new List<Task>();
        
        // Wait for setup countdown to be initiated
        if (!props.StartInstantly)
            await ctx.WaitForExternalEvent(DurableEvents.START_TIMER);

        // Start countdown        
        using var timeoutCts = new CancellationTokenSource();
        var expiration = ctx.CurrentUtcDateTime.AddSeconds(props.Duration);
        var timeoutTask = ctx.CreateTimer(expiration, timeoutCts.Token);

        if (props.Duration != 0)
            tasks.Add(timeoutTask);

        // Wait for external events or timeout expiring
        var restartTask = ctx.WaitForExternalEvent(DurableEvents.RESTART_TIMER);
        tasks.Add(restartTask);
        var resetTask = ctx.WaitForExternalEvent(DurableEvents.RESET_TIMER);
        tasks.Add(resetTask);
        var skipTask = ctx.WaitForExternalEvent(DurableEvents.SKIP_TIMER);
        tasks.Add(skipTask);
        var cancelTask = ctx.WaitForExternalEvent(DurableEvents.CANCEL_TIMER);
        tasks.Add(cancelTask);

        var winner = await Task.WhenAny(tasks);

        if (!timeoutTask.IsCompleted)
            timeoutCts.Cancel();
        
        // Restart countdown if needed
        if (winner == restartTask)
            ctx.ContinueAsNew(new TimerOrchestratorFunctionProps(props.Duration, true));

        if (winner == resetTask)
            ctx.ContinueAsNew(new TimerOrchestratorFunctionProps(props.Duration, false));
    }
}
