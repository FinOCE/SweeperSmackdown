using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Boards;

public static class BoardGetAllFunction
{
    [FunctionName(nameof(BoardGetAllFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "lobbies/{lobbyId}/boards")] HttpRequest req,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? lobby,
        [CosmosDB(
            containerName: DatabaseConstants.PLAYER_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            SqlQuery = "SELECT * FROM c WHERE c.lobbyId = {lobbyId}",
            Connection = "CosmosDbConnectionString")]
            IEnumerable<Player> players,
        [DurableClient] IDurableEntityClient entityClient)
    {
        // Ensure request is from logged in user
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Check if lobby and exists
        if (lobby == null)
            return new NotFoundResult();

        // Check if requester is a lobby member
        if (!players.Any(p => p.Id == requesterId))
            return new StatusCodeResult(403);

        // Check if board exists
        var entityTasks = players.Select(p => KeyValuePair.Create(
            p.Id,
            entityClient.ReadEntityStateAsync<Board>(Id.For<Board>(p.Id))));

        await Task.WhenAll(entityTasks.Select(kvp => kvp.Value));

        var data = entityTasks
            .Select(kvp => KeyValuePair.Create(kvp.Key, kvp.Value.Result))
            .Where(kvp => kvp.Value.EntityExists)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => new BinaryData(kvp.Value.EntityState.GameState).ToString());

        // Respond to request
        return new OkObjectResult(data);
    }
}
