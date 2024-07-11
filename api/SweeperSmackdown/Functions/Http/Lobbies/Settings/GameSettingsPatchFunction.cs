using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Bindings.Entity;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies.Settings;

public static class GameSettingsPatchFunction
{
    [FunctionName(nameof(GameSettingsPatchFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "lobbies/{lobbyId}/settings")] GameSettingsUpdateRequest payload,
        HttpRequest req,
        [Entity("{lobbyId}")] LobbyStateMachine? lobby,
        [Entity("{lobbyId}")] GameSettingsStateMachine? settings,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        // Validate request
        var requesterId = req.GetUserId();

        if (requesterId is null)
            return new StatusCodeResult(401);

        if (lobby is null || settings is null)
            return new NotFoundResult();

        if (!lobby.IsAllowedToModifySettings(requesterId))
            return new StatusCodeResult(403);

        if (!settings.IsConfigurable())
            return new ConflictResult();

        // Update lobby settings
        await entityClient.SignalEntityAsync(
            Id.For<GameSettingsStateMachine>(lobbyId),
            nameof(IGameSettingsStateMachine.UpdateSettings),
            payload);

        // Respond to request
        return new AcceptedResult();
    }
}
