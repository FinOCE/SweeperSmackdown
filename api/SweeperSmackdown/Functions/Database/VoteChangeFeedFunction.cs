using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Database;

public static class VoteChangeFeedFunction
{
    [FunctionName(nameof(VoteChangeFeedFunction))]
    public static async Task Run(
        [CosmosDBTrigger(
            databaseName: DatabaseConstants.DATABASE_NAME,
            containerName: DatabaseConstants.VOTE_CONTAINER_NAME,
            Connection = "CosmosDbConnectionString",
            CreateLeaseContainerIfNotExists = true)]
            Vote vote,
        [CosmosDB(
            containerName: DatabaseConstants.VOTE_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Vote> voteDb,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        await ws.AddAsync(ActionFactory.UpdateVoteState(vote.LobbyId, VoteGroupResponseDto.FromModel(vote)));

        // Handle starting vote
        if (!vote.Triggered && (vote.Votes.Any(kvp => kvp.Value.Length == vote.RequiredVotes) || vote.Forced))
        {
            var choice = vote.Votes.First(kvp => kvp.Value.Length == vote.RequiredVotes).Key;
            var expiry = DateTime.UtcNow.AddSeconds(Constants.SETUP_COUNTDOWN_DURATION);

            await orchestrationClient.RaiseEventAsync(
                Id.ForInstance(nameof(TimerOrchestratorFunction), vote.LobbyId),
                DurableEvents.START_TIMER,
                expiry);

            vote.Triggered = true;
            await voteDb.AddAsync(vote);
            await ws.AddAsync(ActionFactory.StartTimer(vote.LobbyId, expiry));
        }

        // Handle stopping vote
        if (vote.Triggered && (vote.Votes.Any(kvp => kvp.Value.Length == vote.RequiredVotes - 1) || vote.Forced))
        {
            await orchestrationClient.RaiseEventAsync(
                Id.ForInstance(nameof(TimerOrchestratorFunction), vote.LobbyId),
                DurableEvents.RESET_TIMER);

            vote.Triggered = false;
            await voteDb.AddAsync(vote);
            await ws.AddAsync(ActionFactory.ResetTimer(vote.LobbyId));
        }
    }
}
