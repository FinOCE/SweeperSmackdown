using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System.Linq;
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

        // Check if lobby exists
        var lobby = await entityClient.ReadEntityStateAsync<LobbyStateMachine>(
            Id.For<LobbyStateMachine>(lobbyId));

        if (!lobby.EntityExists)
            return new NotFoundResult();

        // Check if user is in lobby
        if (!lobby.EntityState.Players.Any(p => p.Id == requesterId))
            return new StatusCodeResult(403);

        // Get lobby status
        var status = await orchestrationClient.GetStatusAsync(
            Id.ForInstance(nameof(LobbyOrchestratorFunction), lobbyId));

        var customStatus = status.CustomStatus.ToObject<LobbyOrchestratorStatus>();

        if (customStatus is null)
            return new StatusCodeResult(500);

        // Get lobby settings
        var settings = await entityClient.ReadEntityStateAsync<GameSettingsStateMachine>(
            Id.For<GameSettingsStateMachine>(lobbyId));

        if (!settings.EntityExists)
            return new NotFoundResult();

        // Respond to request
        return new OkObjectResult(
            LobbyResponse.FromModel(
                lobbyId,
                customStatus,
                lobby.EntityState,
                settings.EntityState));
    }
}
