using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities.Interactions;

public class GameSettingsUpdateSettingsFailedActivityFunctionProps
{
    public string LobbyId { get; set; }

    public string RequesterId { get; set; }

    public GameSettingsUpdateRequest Updates { get; set; }

    public GameSettingsUpdateSettingsFailedActivityFunctionProps(
        string lobbyId,
        string requesterId,
        GameSettingsUpdateRequest updates)
    {
        LobbyId = lobbyId;
        RequesterId = requesterId;
        Updates = updates;
    }
}

public static class GameSettingsUpdateSettingsFailedActivityFunction
{
    [FunctionName(nameof(GameSettingsUpdateSettingsFailedActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableEntityClient entityClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var input = ctx.GetInput<GameSettingsUpdateSettingsFailedActivityFunctionProps>();

        var settings = await entityClient.ReadEntityStateAsync<GameSettingsStateMachine>(
            Id.For<GameSettingsStateMachine>(input.LobbyId));

        if (!settings.EntityExists)
            throw new InvalidOperationException();

        await ws.AddAsync(ActionFactory.UpdateLobbySettingsFailed(input.RequesterId, settings.EntityState.Settings));
    }
}
