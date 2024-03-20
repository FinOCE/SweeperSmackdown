using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public static class VoteGetAllFunction
{
    [FunctionName(nameof(VoteGetAllFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "/lobbies/{lobbyId}/votes")] HttpRequest req,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        var vote = await entityClient.ReadEntityStateAsync<IVote>(Id.For<Vote>(lobbyId));

        return vote.EntityExists
            ? new OkObjectResult(vote.EntityState.Votes)
            : new NotFoundResult();
    }
}
