using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies.Users;

public static class LobbyUserDeleteFunction
{
    [FunctionName(nameof(LobbyUserDeleteFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "lobbies/{lobbyId}/users/{userId}")] HttpRequest req,
        [DurableClient] IDurableEntityClient entityClient,
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

        // Only allow lobby members to delete
        if (!lobby.EntityState.Players.Any(p => p.Id == requesterId))
            return new StatusCodeResult(403);

        // Only allow the specific user and the host to delete them
        if (requesterId != userId && lobby.EntityState.HostId != userId)
            return new StatusCodeResult(403);

        // Remove user from lobby
        await entityClient.SignalEntityAsync(
            Id.For<LobbyStateMachine>(lobbyId),
            nameof(ILobbyStateMachine.RemovePlayer),
            userId);

        // Respond to request
        return new AcceptedResult();
    }
}
