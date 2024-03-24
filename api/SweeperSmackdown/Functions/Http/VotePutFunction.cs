using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

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
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws,
        string lobbyId,
        string userId)
    {
        // TODO: Get userId for person that made request
        var requesterId = "userId";

        // Check if a vote is in progress
        if (vote == null || lobby == null)
            return new NotFoundResult();

        // Only allow the specific user to delete their vote
        if (lobby.UserIds.Contains(userId) && requesterId != userId)
            return new ForbidResult();

        // Ensure the vote is valid
        if (!vote.Choices.Contains(payload.Choice))
            return new BadRequestObjectResult(new string[]
            {
                $"The 'choice' is not a valid option ({string.Join(", ", vote.Choices)})"
            });

        // Return 200 if old and new choice are the same
        var existingChoice = vote.Votes.Keys.FirstOrDefault(key => vote.Votes[key].Contains(userId));
        if (existingChoice != null && existingChoice == payload.Choice)
            return new OkObjectResult(VoteSingleResponseDto.FromModel(vote, userId));

        // Remove existing vote if present
        if (existingChoice != null)
            vote.Votes[existingChoice] = vote.Votes[existingChoice].Where(id => id != userId).ToArray();

        // Add vote
        vote.Votes[payload.Choice] = vote.Votes[payload.Choice].Append(userId).ToArray();
        
        await voteDb.AddAsync(vote);
        await ws.AddAsync(ActionFactory.AddVote(userId, lobbyId, payload.Choice));

        // Respond to request
        return new CreatedResult(
            $"/lobbies/{lobbyId}/votes/{userId}",
            VoteSingleResponseDto.FromModel(vote, userId));
    }
}
