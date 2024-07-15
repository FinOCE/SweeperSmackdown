using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyPatchFunction
{
    [FunctionName(nameof(LobbyPatchFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "lobbies/{lobbyId}")] LobbyPatchRequest payload,
        HttpRequest req,
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
        if (payload.HostId is not null)
            await entityClient.SignalEntityAsync(
                Id.For<LobbyStateMachine>(lobbyId),
                nameof(ILobbyStateMachine.SetHost),
                payload.HostId);

        if (payload.HostManaged is not null)
            await entityClient.SignalEntityAsync(
                Id.For<LobbyStateMachine>(lobbyId),
                nameof(ILobbyStateMachine.SetHostManaged),
                payload.HostManaged);

        // Respond to request
        return new AcceptedResult();
    }
}
