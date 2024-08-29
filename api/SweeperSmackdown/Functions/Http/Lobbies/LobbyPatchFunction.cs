using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Orchestrators.Requests.Lobbies;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyPatchFunction
{
    [FunctionName(nameof(LobbyPatchFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "lobbies/{lobbyId}")] LobbyPatchRequest payload,
        HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        string lobbyId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Start orchestrator to update lobby
        try
        {
            await orchestrationClient.StartNewAsync(
                nameof(LobbyUpdateFunction),
                new LobbyUpdateFunctionProps(lobbyId, requesterId, payload.HostId, payload.HostManaged));

            // TODO: Poll orchestration and return 200 result

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
