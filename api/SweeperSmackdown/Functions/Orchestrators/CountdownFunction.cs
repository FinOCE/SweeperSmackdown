using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

[DataContract]
public class CountdownFunctionProps
{
    [DataMember]
    public string InstanceId { get; }

    [DataMember]
    public int Duration { get; }

    public CountdownFunctionProps(string instanceId, int duration)
    {
        InstanceId = instanceId;
        Duration = duration;
    }
}

public static class CountdownFunction
{
    [FunctionName(nameof(CountdownFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var props = ctx.GetInput<CountdownFunctionProps>();

        // Wait for setup countdown to be initiated
        await ctx.WaitForExternalEvent(DurableEvents.START_COUNTDOWN);

        // Start countdown
        using var timeoutCts = new CancellationTokenSource();
        
        var expiration = ctx.CurrentUtcDateTime.AddSeconds(props.Duration);
        var timeoutTask = ctx.CreateTimer(expiration, timeoutCts.Token);
        var cancelTask = ctx.WaitForExternalEvent(DurableEvents.CANCEL_COUNTDOWN);
        var skipTask = ctx.WaitForExternalEvent(DurableEvents.SKIP_COUNTDOWN);

        var winner = await Task.WhenAny(timeoutTask, cancelTask, skipTask);

        if (!timeoutTask.IsCompleted)
            timeoutCts.Cancel();

        // Restart countdown if cancelled
        if (winner == cancelTask)
            ctx.ContinueAsNew(props);
    }
}
