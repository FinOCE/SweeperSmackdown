using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public static class VoteDeleteFunction
{
    [FunctionName(nameof(VoteDeleteFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "/lobbies/{lobbyId}/votes/{userId}")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId,
        string userId,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> actions)
    {
        // Check if a vote is in progress
        var vote = await entityClient.ReadEntityStateAsync<Vote>(Id.For<Vote>(lobbyId));

        if (!vote.EntityExists)
            return new NotFoundResult();

        // Indicate if vote didn't exist
        KeyValuePair<string, string[]> kvp;
        try
        {
            kvp = vote.EntityState.Votes.First(kvp => kvp.Value.Contains(userId));
        }
        catch (Exception)
        {
            return new NotFoundResult();
        }

        // Remove user vote
        await entityClient.SignalEntityAsync<IVote>(
            Id.For<Vote>(lobbyId),
            vote => vote.RemoveVote(userId));

        await actions.AddAsync(ActionFactory.RemoveVote(userId, lobbyId, kvp.Key));

        // Notify voted item if dropped below required votes
        vote = await entityClient.ReadEntityStateAsync<Vote>(Id.For<Vote>(lobbyId));
        
        if (vote.EntityState.Votes[kvp.Key].Length == vote.EntityState.RequiredVotes - 1)
        {
            var lobby = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));

            switch (lobby.EntityState.Status)
            {
                case ELobbyStatus.Setup:
                    await orchestrationClient.RaiseEventAsync(
                        Id.ForInstance(nameof(CountdownFunction), lobbyId),
                        DurableEvents.CANCEL_COUNTDOWN);
                    break;
            }
        }
        
        // Return no content
        return new NoContentResult();
    }
}
