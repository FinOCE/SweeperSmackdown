using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class GameCelebrationNotifyCountdownActivityFunctionProps
{
    public string LobbyId { get; set; }

    public DateTime Expiry { get; set; }

    public GameCelebrationNotifyCountdownActivityFunctionProps(string lobbyId, DateTime expiry)
    {
        LobbyId = lobbyId;
        Expiry = expiry;
    }
}

public static class GameCelebrationNotifyCountdownActivityFunction
{
    [FunctionName(nameof(GameCelebrationNotifyCountdownActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var props = ctx.GetInput<GameCelebrationNotifyCountdownActivityFunctionProps>();

        // Notify clients of game starting
        await ws.AddAsync(ActionFactory.GameCelebrationStarting(props.LobbyId, props.Expiry));
    }
}
