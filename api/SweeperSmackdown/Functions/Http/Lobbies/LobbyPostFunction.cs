using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyPostFunction
{
    [FunctionName(nameof(LobbyPostFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "lobbies")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId is null)
            return new StatusCodeResult(401);

        // Generate unique lobby ID
        var lobbyId = "";
        var existing = true;

        do
        {
            lobbyId = RandomNumberGenerator.GetInt32(100_000).ToString();

            var lobby = await entityClient.ReadEntityStateAsync<LobbyStateMachine>(
                Id.For<LobbyStateMachine>(lobbyId));

            existing = lobby.EntityExists;
        }
        while (existing);

        // Start create orchestrator and return 202
        await orchestrationClient.StartNewAsync(
            nameof(LobbyCreateOrchestratorFunction),
            Id.ForInstance(nameof(LobbyCreateOrchestratorFunction), lobbyId),
            new LobbyCreateOrchestratorFunctionProps(requesterId));

        return new AcceptedResult();
    }
}
