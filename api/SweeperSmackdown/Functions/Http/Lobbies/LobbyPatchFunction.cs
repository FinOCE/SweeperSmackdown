using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyPatchFunction
{
    [FunctionName(nameof(LobbyPatchFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "lobbies/{lobbyId}")] LobbyPatchRequest payload,
        HttpRequest req,
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

        // Only allow host to modify
        if (lobby.EntityState.HostId != requesterId)
            return new StatusCodeResult(403);

        // Signal entity to update provided values
        await entityClient.SignalEntityAsync(
            Id.For<LobbyStateMachine>(lobbyId),
            nameof(ILobbyStateMachine.Set),
            (payload.HostId, payload.HostManaged));

        var updatedLobby = await entityClient.WaitForUpdateAsync<LobbyStateMachine>(
            Id.For<LobbyStateMachine>(lobbyId),
            lobby =>
                (lobby.HostId == (payload.HostId ?? lobby.HostId)) &&
                (lobby.HostManaged == (payload.HostManaged ?? lobby.HostManaged)));

        // Fetch state for response
        var settings = await entityClient.ReadEntityStateAsync<GameSettingsStateMachine>(
                Id.For<GameSettingsStateMachine>(lobbyId));

        if (!settings.EntityExists)
            return new StatusCodeResult(500);

        var status = await orchestrationClient.GetStatusAsync(
            Id.ForInstance(nameof(LobbyOrchestratorFunction), lobbyId));

        var customStatus = status.CustomStatus.ToObject<LobbyOrchestratorStatus>();

        if (customStatus is null)
            return new StatusCodeResult(500);

        // Respond to request
        return new OkObjectResult(
            LobbyResponse.FromModel(
                lobbyId,
                new PreciseLobbyStatus(customStatus, customStatus.Status == ELobbyStatus.Configuring ? settings.EntityState.State : null),
                updatedLobby,
                settings.EntityState));
    }
}
