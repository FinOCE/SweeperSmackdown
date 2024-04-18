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

public static class VotePutFunction
{
    [FunctionName(nameof(VotePutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}/votes/{userId}")] VotePutRequestDto payload,
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

        // Reject force from non-hosts
        if (lobby.HostId != requesterId && payload.Force != null && payload.Force.Value)
            return new StatusCodeResult(403);

        // Ensure the vote is valid        
        if (!vote.Choices.Contains(payload.Choice))
            return new BadRequestObjectResult(new string[]
            {
                $"The 'choice' is not a valid option ({string.Join(", ", vote.Choices)})"
            });

        // Prevent votes if voting is forced
        if (vote.Forced)
            return new StatusCodeResult(403);

        // Handle vote forcing
        if (forced)
            vote.Forced = true;

        // Remove existing vote if present and add new vote
        var existingChoice = vote.Votes.Keys.FirstOrDefault(key => vote.Votes[key].Contains(userId));

        if (existingChoice != null)
            vote.Votes[existingChoice] = vote.Votes[existingChoice].Where(id => id != userId).ToArray();

        if (existingChoice != null || existingChoice != payload.Choice)
            vote.Votes[payload.Choice] = vote.Votes[payload.Choice].Append(userId).ToArray();

        // Update database
        await voteDb.AddAsync(vote);

        // Respond to request
        return existingChoice != null && existingChoice == payload.Choice
            ? new OkObjectResult(VoteSingleResponseDto.FromModel(vote, userId))
            : new CreatedResult(
                $"/lobbies/{lobbyId}/votes/{userId}",
                VoteSingleResponseDto.FromModel(vote, userId));
    }
}
