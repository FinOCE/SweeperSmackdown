using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Orchestrators.Requests.Lobbies;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyDeleteFunction
{
    [FunctionName(nameof(LobbyDeleteFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "lobbies/{lobbyId}")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        string lobbyId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Start orchestrator to delete lobby
        try
        {
            await orchestrationClient.StartNewAsync(
                nameof(LobbyDisposeFunction),
                new LobbyDisposeFunctionProps(lobbyId, requesterId));

            // TODO: Poll orchestration and return 204 result

            return new AcceptedResult();
        }
        catch (FunctionFailedException ex)
        {
            if (ex.InnerException is AccessViolationException)
                return new StatusCodeResult(403);

            if (ex.InnerException is ArgumentException)
                return new StatusCodeResult(404);

            return new StatusCodeResult(500);
        }
    }
}
