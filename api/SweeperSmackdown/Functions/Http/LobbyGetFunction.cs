using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Models;
using System.Linq;

namespace SweeperSmackdown.Functions.Http;

public static class LobbyGetFunction
{
    [FunctionName(nameof(LobbyGetFunction))]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "lobbies/{lobbyId}")] HttpRequest _,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? lobby)
    {
        // TODO: Get userId for person that made request
        var requesterId = "userId";
        
        if (lobby == null)
            return new NotFoundResult();

        if (!lobby.UserIds.Contains(requesterId))
            return new ForbidResult();
        
        return new OkObjectResult(LobbyResponseDto.FromModel(lobby));
    }
}
