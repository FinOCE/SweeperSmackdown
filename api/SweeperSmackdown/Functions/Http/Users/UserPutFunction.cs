using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Users;

public static class UserPutFunction
{
    [FunctionName(nameof(UserPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}/users/{userId}")] HttpRequest req,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? lobby,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Lobby> lobbyDb,
        string lobbyId,
        string userId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Check if lobby exists and that user is in it
        if (lobby == null)
            return new NotFoundResult();

        // Only allow the specific user join themselves
        if (requesterId != userId)
            return new StatusCodeResult(403);

        // Add to lobby
        var alreadyInLobby = lobby.UserIds.Contains(userId);
        lobby.UserIds = lobby.UserIds.Append(userId).Distinct().ToArray();
        await lobbyDb.AddAsync(lobby);

        // Respond to request
        return alreadyInLobby
            ? new OkObjectResult(UserResponseDto.FromModel(lobby, userId))
            : new CreatedResult(
                $"/lobbies/{lobbyId}/users/{userId}",
                UserResponseDto.FromModel(lobby, userId));
    }
}
