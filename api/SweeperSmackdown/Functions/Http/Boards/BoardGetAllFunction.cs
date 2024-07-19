using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Boards;

public static class BoardGetAllFunction
{
    [FunctionName(nameof(BoardGetAllFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "lobbies/{lobbyId}/boards")] HttpRequest req,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
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

        // Check if requester is a lobby member
        if (!lobby.EntityState.Players.Any(p => p.Id == requesterId))
            return new StatusCodeResult(403);

        // Check if board exists
        var entityTasks = lobby.EntityState.Players.Select(p => KeyValuePair.Create(
            p.Id,
            entityClient.ReadEntityStateAsync<Board>(Id.For<Board>(p.Id))));

        await Task.WhenAll(entityTasks.Select(kvp => kvp.Value));

        var data = entityTasks
            .Select(kvp => KeyValuePair.Create(kvp.Key, kvp.Value.Result))
            .Where(kvp => kvp.Value.EntityExists)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => PlayerState.FromEntity(kvp.Value.EntityState));

        // Respond to request
        return new OkObjectResult(data);
    }
}
