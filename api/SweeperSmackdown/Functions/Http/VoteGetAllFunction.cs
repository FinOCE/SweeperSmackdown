using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Models;
using System.Linq;

namespace SweeperSmackdown.Functions.Http;

public static class VoteGetAllFunction
{
    [FunctionName(nameof(VoteGetAllFunction))]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "lobbies/{lobbyId}/votes")] HttpRequest req,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? lobby,
        [CosmosDB(
            containerName: DatabaseConstants.VOTE_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Vote? vote)
    {
        // TODO: Get userId for person that made request
        var requesterId = "userId";
        
        if (vote == null || lobby == null)
            return new NotFoundResult();

        if (!lobby.UserIds.Contains(requesterId))
            return new ForbidResult();

        return new OkObjectResult(VoteGroupResponseDto.FromModel(vote));
    }
}
