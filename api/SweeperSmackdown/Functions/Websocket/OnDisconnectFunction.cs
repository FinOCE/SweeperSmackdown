using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Websocket;

public static class OnDisconnectFunction
{
    [FunctionName(nameof(OnDisconnectFunction))]
    public static async Task Run(
        [WebPubSubTrigger(PubSubConstants.HUB_NAME, WebPubSubEventType.System, "disconnected")] DisconnectedEventRequest req,
        [DurableClient] IDurableEntityClient entityClient)
    {
        var userId = req.ConnectionContext.UserId;

        var reference = await entityClient.ReadEntityStateAsync<ConnectionReference>(
            Id.For<ConnectionReference>(userId));

        if (!reference.EntityExists)
            return;

        var lobbyId = reference.EntityState.LobbyId;

        if (lobbyId is not null)
            await entityClient.SignalEntityAsync(
                Id.For<LobbyStateMachine>(lobbyId),
                nameof(ILobbyStateMachine.RemovePlayer),
                userId);

        await entityClient.SignalEntityAsync(
            Id.For<ConnectionReference>(userId),
            nameof(IConnectionReference.Delete));
    }
}
