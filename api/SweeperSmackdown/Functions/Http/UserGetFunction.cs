using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using System.Linq;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Models;

namespace SweeperSmackdown.Functions.Http;

public static class UserGetFunction
{
    [FunctionName(nameof(UserGetFunction))]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "lobbies/{lobbyId}/users/{userId}")] HttpRequest _,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "%CosmosDbConnectionString%",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? lobby,
        string userId)
    {
        if (lobby == null)
            return new NotFoundResult();

        if (!lobby.UserIds.Contains(userId))
            return new NotFoundResult();

        return new OkObjectResult(UserResponseDto.FromModel(lobby, userId));
    }
}
