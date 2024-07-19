using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies.Settings;

public static class GameSettingsPatchFunction
{
    [FunctionName(nameof(GameSettingsPatchFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "lobbies/{lobbyId}/settings")] GameSettingsUpdateRequest payload,
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

        // Only allow lobby members to modify
        if (!lobby.EntityState.Players.Any(p => p.Id == requesterId))
            return new StatusCodeResult(403);

        // Only allow host to modify if host managed
        if (lobby.EntityState.HostId != requesterId && lobby.EntityState.HostManaged)
            return new StatusCodeResult(403);

        // Short circuit if entity is in invalid state
        var settings = await entityClient.ReadEntityStateAsync<GameSettingsStateMachine>(
            Id.For<GameSettingsStateMachine>(lobbyId));

        if (!settings.EntityExists)
            return new NotFoundResult();

        if (!GameSettingsStateMachine.ValidStatesToUpdateSettings.Contains(settings.EntityState.State))
            return new ConflictResult();

        // Update lobby settings
        await entityClient.SignalEntityAsync(
            Id.For<GameSettingsStateMachine>(lobbyId),
            nameof(IGameSettingsStateMachine.UpdateSettings),
            payload);

        return new AcceptedResult();
    }
}
