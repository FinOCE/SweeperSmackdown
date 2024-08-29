using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Orchestrators.Requests.Lobbies;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyPostFunction
{
    [FunctionName(nameof(LobbyPostFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "lobbies")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId is null)
            return new StatusCodeResult(401);

        // Create lobby
        await orchestrationClient.StartNewAsync(
            nameof(LobbyCreateFunction),
            new LobbyCreateFunctionProps(null, requesterId));

        // TODO: Poll orchestration and return 201 result

        return new AcceptedResult();
    }
}
