using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyPatchFunction
{
    [FunctionName(nameof(LobbyPatchFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "lobbies/{lobbyId}")] LobbyPatchRequestDto payload,
        HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? lobby,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Lobby> db,
        string lobbyId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Handle validation failures
        if (!payload.IsValid)
            return new BadRequestObjectResult(payload.Errors);

        // Check if lobby exists or is not in configure state
        if (lobby == null)
            return new NotFoundResult();

        var orchestrationStatus = await orchestrationClient.GetStatusAsync(
            Id.ForInstance(nameof(LobbyOrchestratorFunction), lobbyId));

        if (orchestrationStatus != null)
        {
            var customStatus = orchestrationStatus.CustomStatus.ToString();
            var status = Enum.Parse<ELobbyOrchestratorFunctionStatus>(customStatus);

            // TODO: Fix this so it doesn't allow requests after countdown rather than any point during configuration
            //       (edge case, not urgent, but still a potential issue)

            if (status != ELobbyOrchestratorFunctionStatus.Configure)
                return new ConflictResult();
        }

        // Only allow lobby members to modify
        if (!lobby.UserIds.Contains(requesterId))
            return new StatusCodeResult(403);

        // Confirm mine count is realistic
        var newHeight = payload.Height ?? lobby.Settings.Height;
        var newWidth = payload.Width ?? lobby.Settings.Width;
        var newMines = payload.Mines ?? lobby.Settings.Mines;

        if (newHeight * newWidth < newMines)
            return new BadRequestObjectResult(new string[]
            {
                "Cannot update because this would result in more mines than there are board squares"
            });

        // Confirm user is allowed to change host ID if attempted
        if (payload.HostId != null && lobby.HostId != requesterId)
            return new BadRequestObjectResult(new string[]
            {
                "Cannot change host ID because you are not the current host"
            });

        // Update lobby settings
        int? seed = null;

        if (payload.ShareBoards != null)
            seed = payload.ShareBoards.Value
                ? Guid.NewGuid().GetHashCode()
                : 0;

        lobby.HostId = payload.HostId ?? lobby.HostId;
        lobby.Settings = lobby.Settings.Update(
            payload.Mode,
            payload.Height,
            payload.Width,
            payload.Mines,
            payload.Lives,
            payload.TimeLimit,
            payload.BoardCount,
            seed);

        await db.AddAsync(lobby);

        // Respond to request
        return new OkObjectResult(LobbyResponseDto.FromModel(lobby));
    }
}
