using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Boards;

public static class BoardResetActionFunction
{
    [FunctionName(nameof(BoardResetActionFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post",
            Route = "lobbies/{lobbyId}/boards/{userId}/reset")]
        HttpRequest req,
        [DurableClient] IDurableEntityClient entityClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws,
        string lobbyId,
        string userId)
    {
        // Ensure request is from logged in user
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Check if lobby exists
        var lobby = await entityClient.ReadEntityStateAsync<LobbyStateMachine>(
            Id.For<LobbyStateMachine>(lobbyId));

        if (!lobby.EntityExists)
            return new NotFoundResult();

        // Check if requester is a lobby member and the user specified
        if (!lobby.EntityState.Players.Any(p => p.Id == requesterId) || requesterId != userId)
            return new StatusCodeResult(403);

        // Check if board exists
        var entity = await entityClient.ReadEntityStateAsync<Board>(Id.For<Board>(userId));

        if (!entity.EntityExists)
            return new NotFoundResult();

        if (entity.EntityState.IsDisabled)
            return new ConflictResult();

        // Reset board
        await entityClient.SignalEntityAsync<IBoard>(
            Id.For<Board>(userId),
            board => board.Reset());

        await ws.AddAsync(ActionFactory.CreateBoard(userId, lobbyId, entity.EntityState.InitialState, true));

        // Respond to request
        return new AcceptedResult();
    }
}
