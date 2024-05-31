using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyUnlockActionFunction
{
    [FunctionName(nameof(LobbyUnlockActionFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", "lobbies/{lobbyId}/unlock")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? lobby)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Check if lobby exists or is not in configure state
        if (lobby == null)
            return new NotFoundResult();

        if (lobby.State != ELobbyState.ConfigureLocked)
            return new ConflictResult();

        // Only allow lobby host to modify
        if (lobby.HostId != requesterId)
            return new StatusCodeResult(403);

        // Raise event
        await orchestrationClient.RaiseEventAsync(
            Id.ForInstance(nameof(GameConfigureFunction), lobby.Id),
            DurableEvents.GAME_START_UNLOCK);

        return new NoContentResult();
    }
}
