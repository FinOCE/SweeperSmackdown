using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
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
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
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

        // Notify orchestration
        if (vote.Votes[payload.Choice].Length == vote.RequiredVotes || forced)
        {
            var expiry = DateTime.UtcNow.AddSeconds(Constants.SETUP_COUNTDOWN_DURATION);

            await orchestrationClient.RaiseEventAsync(
                Id.ForInstance(nameof(TimerOrchestratorFunction), lobbyId),
                DurableEvents.START_TIMER,
                expiry);
            
            await ws.AddAsync(ActionFactory.StartTimer(lobbyId, expiry));
        }

        // Update database and notify users of vote state change
        await voteDb.AddAsync(vote);
        await ws.AddAsync(ActionFactory.UpdateVoteState(lobbyId, VoteGroupResponseDto.FromModel(vote)));

        // Respond to request
        return existingChoice != null && existingChoice == payload.Choice
            ? new OkObjectResult(VoteSingleResponseDto.FromModel(vote, userId))
            : new CreatedResult(
                $"/lobbies/{lobbyId}/votes/{userId}",
                VoteSingleResponseDto.FromModel(vote, userId));
    }
}
