using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
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
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "/lobbies/{lobbyId}/votes/{userId}")] HttpRequest req,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId,
        string userId)
    {
        // Check if a vote is in progress
        var vote = await entityClient.ReadEntityStateAsync<Vote>(Id.For<Vote>(lobbyId));

        if (!vote.EntityExists)
            return new NotFoundResult();
        
        try
        {
            var kvp = vote.EntityState.Votes.First(kvp => kvp.Value.Contains(userId));
            return new OkObjectResult(new { choice = kvp.Key });
        }
        catch (Exception)
        {
            return new NotFoundResult();
        }
    }
}
