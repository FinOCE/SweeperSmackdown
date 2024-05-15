using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class TimerCompletedActivityFunctionProps
{
    public string LobbyId { get; set; }
    
    public string RaiseEventTo { get; set; }

    public TimerCompletedActivityFunctionProps(string lobbyId, string raiseEventTo)
    {
        LobbyId = lobbyId;
        RaiseEventTo = raiseEventTo;
    }
}

public static class TimerCompletedActivityFunction
{
    [FunctionName(nameof(TimerCompletedActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var props = ctx.GetInput<TimerCompletedActivityFunctionProps>();
        
        await orchestrationClient.RaiseEventAsync(
                props.RaiseEventTo,
                DurableEvents.TIMER_COMPLETED);
        
        await ws.AddAsync(ActionFactory.ClearTimer(props.LobbyId));
    }
}
