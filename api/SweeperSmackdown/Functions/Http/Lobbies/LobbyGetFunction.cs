using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Orchestrators.Requests.Lobbies;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyGetFunction
{
    [FunctionName(nameof(LobbyGetFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "lobbies/{lobbyId}")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Fetch lobby
        await orchestrationClient.StartNewAsync(
            nameof(LobbyFetchFunction),
            new LobbyFetchFunctionProps(lobbyId));

        // TODO: Poll orchestration and return 200 or 404 result

        return new AcceptedResult();
    }
}
