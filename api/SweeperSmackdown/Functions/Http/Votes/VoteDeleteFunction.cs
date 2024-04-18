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

namespace SweeperSmackdown.Functions.Http.Votes;

public static class VoteDeleteFunction
{
    [FunctionName(nameof(VoteDeleteFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "lobbies/{lobbyId}/votes/{userId}")] VoteDeleteRequestDto payload,
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
            Vote? vote,
        [CosmosDB(
            containerName: DatabaseConstants.VOTE_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Vote> voteDb,
        HttpRequest req,
        string lobbyId,
        string userId)
    {
        var forced = payload.Force != null && payload.Force.Value;

        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Check if a vote is in progress
        if (vote == null || lobby == null)
            return new NotFoundResult();

        // Only allow the specific user to delete their vote
        if (!lobby.UserIds.Contains(userId) || requesterId != userId)
            return new StatusCodeResult(403);

        // Prevent removing votes if vote is forced
        if (vote.Forced && !forced)
            return new StatusCodeResult(403);

        // Only allow removal if user has a vote
        var hasExistingChoice = vote.Votes.Any(kvp => kvp.Value.Contains(userId));

        if (!forced && !hasExistingChoice)
            return new NotFoundResult();

        // Remove user vote
        string? choice = null;
        if (hasExistingChoice)
        {
            choice = vote.Votes.First(kvp => kvp.Value.Contains(userId)).Key;
            vote.Votes[choice] = vote.Votes[choice].Where(id => id != userId).ToArray();
        }

        if (forced)
        {
            vote.Forced = false;

            foreach (var key in vote.Votes.Keys)
                vote.Votes[key] = Array.Empty<string>();
        }

        // Update database and notify users of vote state change
        await voteDb.AddAsync(vote);

        // Respond to request
        return new NoContentResult();
    }
}
