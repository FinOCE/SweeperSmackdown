using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators.Interactions;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies.Settings;

public static class GameSettingsPatchFunction
{
    [FunctionName(nameof(GameSettingsPatchFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "lobbies/{lobbyId}/settings")] GameSettingsUpdateRequest payload,
        HttpRequest req,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? lobby,
        [CosmosDB(
            containerName: DatabaseConstants.PLAYER_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            SqlQuery = "SELECT * FROM c WHERE c.lobbyId = {lobbyId}",
            Connection = "CosmosDbConnectionString")]
            IEnumerable<Player> players,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Check if lobby exists or is not in configure state
        if (lobby == null)
            return new NotFoundResult();

        // Only allow lobby members to modify
        if (!players.Any(p => p.Id == requesterId))
            return new StatusCodeResult(403);

        // Only allow host to modify if host managed
        if (lobby.HostId != requesterId && lobby.HostManaged)
            return new StatusCodeResult(403);

        // Short circuit if entity is in invalid state
        var settings = await entityClient.ReadEntityStateAsync<GameSettingsStateMachine>(
            Id.For<GameSettingsStateMachine>(lobbyId));

        if (!settings.EntityExists)
            return new NotFoundResult();

        if (!GameSettingsStateMachine.ValidStatesToUpdateSettings.Contains(settings.EntityState.State))
            return new ConflictResult();

        // Start settings update workflow
        await orchestrationClient.StartNewAsync(
            nameof(GameSettingsUpdateOrchestratorFunction),
            lobbyId,
            new GameSettingsUpdateOrchestratorFunctionProps(requesterId, payload));

        return new AcceptedResult();
    }
}
