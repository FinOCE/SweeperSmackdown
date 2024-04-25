using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class TimerCompletedActivityFunctionProps
{
    public string RaiseEventTo { get; set; }

    public TimerCompletedActivityFunctionProps(string raiseEventTo)
    {
        RaiseEventTo = raiseEventTo;
    }
}

public static class TimerCompletedActivityFunction
{
    [FunctionName(nameof(TimerCompletedActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableOrchestrationClient orchestrationClient)
    {
        var props = ctx.GetInput<TimerCompletedActivityFunctionProps>();
        
        await orchestrationClient.RaiseEventAsync(
                props.RaiseEventTo,
                DurableEvents.TIMER_COMPLETED);
    }
}
