using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Models;
using SweeperSmackdown.Factories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public static class VoteDeleteFunction
{
    [FunctionName(nameof(VoteDeleteFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "lobbies/{lobbyId}/votes/{userId}")] HttpRequest _,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "%CosmosDbConnectionString%",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? lobby,
        [CosmosDB(
            containerName: DatabaseConstants.VOTE_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "%CosmosDbConnectionString%",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Vote? vote,
        [CosmosDB(
            containerName: DatabaseConstants.VOTE_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "%CosmosDbConnectionString%")]
            IAsyncCollector<Vote> voteDb,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws,
        string lobbyId,
        string userId)
    {
        // TODO: Get userId for person that made request
        var requesterId = "userId";

        // Check if a vote is in progress and that the user has a vote
        if (vote == null || vote.Votes.Any(kvp => kvp.Value.Contains(userId)) || lobby == null)
            return new NotFoundResult();

        // Only allow the specific user to delete their vote
        if (lobby.UserIds.Contains(userId) && requesterId != userId)
            return new ForbidResult();

        // Remove user vote
        var choice = vote.Votes.First(kvp => kvp.Value.Contains(userId)).Key;
        vote.Votes[choice] = vote.Votes[choice].Where(id => id != userId).ToArray();

        await voteDb.AddAsync(vote);
        await ws.AddAsync(ActionFactory.RemoveVote(userId, lobbyId, choice));

        // Respond to request
        return new NoContentResult();
    }
}
