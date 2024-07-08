using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies.Users;

public static class LobbyUserPutFunction
{
    [FunctionName(nameof(LobbyUserPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}/users/{userId}")] HttpRequest req,
        [DurableClient] IDurableEntityClient entityClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws,
        string lobbyId,
        string userId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Check if lobby exists
        var lobby = await entityClient.ReadEntityStateAsync<LobbyStateMachine>(
            Id.For<LobbyStateMachine>(lobbyId));

        if (!lobby.EntityExists)
            return new NotFoundResult();

        // Only allow the specific user join themselves
        if (requesterId != userId)
            return new StatusCodeResult(403);

        // Add player to lobby
        await entityClient.SignalEntityAsync(
            Id.For<LobbyStateMachine>(lobbyId),
            nameof(ILobbyStateMachine.AddPlayer),
            userId);

        // Respond to request
        return new AcceptedResult();
    }
}
