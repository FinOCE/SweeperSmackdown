using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyPutFunction
{
    [FunctionName(nameof(LobbyPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId is null)
            return new StatusCodeResult(401);

        // Check if lobby already exists
        var lobby = await entityClient.ReadEntityStateAsync<LobbyStateMachine>(
            Id.For<LobbyStateMachine>(lobbyId));

        if (lobby.EntityExists)
        {
            // Fetch remaining data and return 200
            var settings = await entityClient.ReadEntityStateAsync<GameSettingsStateMachine>(
                Id.For<GameSettingsStateMachine>(lobbyId));

            if (!settings.EntityExists)
                return new StatusCodeResult(500);

            var status = await orchestrationClient.GetStatusAsync(
                Id.ForInstance(nameof(LobbyOrchestratorFunction), lobbyId));

            var customStatus = status.CustomStatus.ToObject<LobbyOrchestratorStatus>();

            if (customStatus is null)
                return new StatusCodeResult(500);

            return new OkObjectResult(LobbyResponseDto.FromModel(
                lobbyId,
                customStatus,
                lobby.EntityState,
                settings.EntityState));
        }
        else
        {
            // Start create orchestrator and return 202
            await orchestrationClient.StartNewAsync(
                nameof(LobbyCreateOrchestratorFunction),
                Id.ForInstance(nameof(LobbyCreateOrchestratorFunction), lobbyId),
                new LobbyCreateOrchestratorFunctionProps(requesterId));

            return new AcceptedResult();
        }
    }
}
