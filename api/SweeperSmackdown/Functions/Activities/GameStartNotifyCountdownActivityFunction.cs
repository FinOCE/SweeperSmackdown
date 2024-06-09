using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class GameStartNotifyCountdownActivityFunctionProps
{
    public string LobbyId { get; set; }

    public DateTime Expiry { get; set; }

    public GameStartNotifyCountdownActivityFunctionProps(string lobbyId, DateTime expiry)
    {
        LobbyId = lobbyId;
        Expiry = expiry;
    }
}

public static class GameStartNotifyCountdownActivityFunction
{
    [FunctionName(nameof(GameStartNotifyCountdownActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var props = ctx.GetInput<GameStartNotifyCountdownActivityFunctionProps>();

        // Notify clients of game starting
        await ws.AddAsync(ActionFactory.GameStarting(props.LobbyId, props.Expiry));
    }
}
