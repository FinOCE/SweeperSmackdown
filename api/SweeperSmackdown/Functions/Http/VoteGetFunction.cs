using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public static class VoteGetFunction
{
    [FunctionName(nameof(VoteGetFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "lobbies/{lobbyId}/votes/{userId}")] HttpRequest req,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId,
        string userId)
    {
        // Check if a vote is in progress and the user has votes
        var vote = await entityClient.ReadEntityStateAsync<Vote>(Id.For<Vote>(lobbyId));

        if (!vote.EntityExists || !vote.EntityState.Votes.Any(kvp => kvp.Value.Contains(userId)))
            return new NotFoundResult();
        
        // Get their vote and return
        var kvp = vote.EntityState.Votes.First(kvp => kvp.Value.Contains(userId));
        return new OkObjectResult(new VoteSingleResponseDto(lobbyId, userId, kvp.Key));
    }
}
