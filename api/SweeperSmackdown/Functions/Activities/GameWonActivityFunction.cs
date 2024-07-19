using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class GameWonActivityFunctionProps
{
    public string LobbyId { get; set; }

    public string WinnerId { get; set; }

    public GameWonActivityFunctionProps(string lobbyId, string winnerId)
    {
        LobbyId = lobbyId;
        WinnerId = winnerId;
    }
}

public static class GameWonActivityFunction
{
    [FunctionName(nameof(GameWonActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var props = ctx.GetInput<GameWonActivityFunctionProps>();
        
        // Notify game orchestrator of winner
        await orchestrationClient.RaiseEventAsync(
            Id.ForInstance(nameof(LobbyOrchestratorFunction), props.LobbyId),
            DurableEvents.GAME_WON,
            props.WinnerId);

        // Notify clients of winner
        await ws.AddAsync(ActionFactory.GameWon(props.WinnerId, props.LobbyId));
    }
}
