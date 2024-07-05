using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators.Interactions;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyLockActionFunction
{
    [FunctionName(nameof(LobbyLockActionFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "lobbies/{lobbyId}/lock")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? lobby,
        string lobbyId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Check if lobby exists or is not in configure state
        if (lobby == null)
            return new NotFoundResult();

        // Only allow lobby host to modify
        if (lobby.HostId != requesterId)
            return new StatusCodeResult(403);

        // Short circuit if entity is in invalid state
        var settings = await entityClient.ReadEntityStateAsync<GameSettingsStateMachine>(
            Id.For<GameSettingsStateMachine>(lobbyId));

        if (!settings.EntityExists)
            return new NotFoundResult();

        if (!GameSettingsStateMachine.ValidStatesToLock.Contains(settings.EntityState.State))
            return new ConflictResult();

        // Start lock workflow
        await orchestrationClient.StartNewAsync(
            nameof(GameSettingsLockOrchestratorFunction),
            lobby.Id,
            new GameSettingsLockOrchestratorFunctionProps(requesterId));

        return new AcceptedResult();
    }
}
