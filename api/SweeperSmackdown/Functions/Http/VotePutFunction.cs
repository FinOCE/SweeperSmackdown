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
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public class VotePutFunctionPayload
{
    public string Choice { get; } = null!;
}

public static class VotePutFunction
{
    [FunctionName(nameof(VotePutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "/lobbies/{lobbyId}/votes/{userId}")] VotePutFunctionPayload payload,
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
        
        // Ensure the vote is valid
        if (vote.EntityState.Choices.Contains(payload.Choice))
            return new BadRequestObjectResult(new string[]
            {
                $"The 'choice' is not a valid option ({string.Join(", ", vote.EntityState.Choices)})"
            });

        // Add user vote
        await entityClient.SignalEntityAsync<IVote>(
            Id.For<Vote>(lobbyId),
            vote => vote.AddVote((userId, payload.Choice, lobbyId, orchestrationClient)));

        await actions.AddAsync(ActionFactory.AddVote(userId, lobbyId, payload.Choice));

        // Get updated results and return to user
        return new CreatedResult($"/lobbies/{lobbyId}/votes", vote.EntityState.Votes);
    }
}
