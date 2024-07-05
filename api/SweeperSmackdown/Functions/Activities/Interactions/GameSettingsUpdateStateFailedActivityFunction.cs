using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities.Interactions;

public class GameSettingsUpdateStateFailedActivityFunctionProps
{
    public string LobbyId { get; set; }

    public string RequesterId { get; set; }

    public GameSettingsUpdateStateFailedActivityFunctionProps(
        string lobbyId,
        string requesterId)
    {
        LobbyId = lobbyId;
        RequesterId = requesterId;
    }
}

public static class GameSettingsUpdateStateFailedActivityFunction
{
    [FunctionName(nameof(GameSettingsUpdateStateFailedActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableEntityClient entityClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var input = ctx.GetInput<GameSettingsUpdateStateFailedActivityFunctionProps>();

        var settings = await entityClient.ReadEntityStateAsync<GameSettingsStateMachine>(
            Id.For<GameSettingsStateMachine>(input.LobbyId));

        if (!settings.EntityExists)
            throw new InvalidOperationException();

        await ws.AddAsync(ActionFactory.UpdateLobbyStateFailed(input.RequesterId, settings.EntityState.State));
    }
}
