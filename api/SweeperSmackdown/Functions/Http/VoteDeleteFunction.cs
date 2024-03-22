using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Factories;
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
            vote => vote.RemoveVote((userId, lobbyId, orchestrationClient)));

        await actions.AddAsync(ActionFactory.RemoveVote(userId, lobbyId, kvp.Key));
        
        // Return no content
        return new NoContentResult();
    }
}
