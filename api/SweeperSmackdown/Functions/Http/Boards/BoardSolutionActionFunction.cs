using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Boards;

public static class BoardSolutionActionFunction
{
    [FunctionName(nameof(BoardSolutionActionFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post",
            Route = "lobbies/{lobbyId}/boards/{userId}/solution")]
            BoardSolutionPostRequestDto payload,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? lobby,
        [CosmosDB(
            containerName: DatabaseConstants.BOARD_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            BoardEntityMap? boardEntityMap,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        HttpRequest req,
        string lobbyId,
        string userId)
    {
        // Ensure request is from logged in user
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Check if lobbby and board exist
        if (lobby == null || boardEntityMap == null || !boardEntityMap.BoardIds.Contains(userId))
            return new NotFoundResult();

        // Check if requester is the user
        if (!lobby.UserIds.Contains(requesterId) || requesterId != userId)
            return new StatusCodeResult(403);

        // Check if board exists
        var entity = await entityClient.ReadEntityStateAsync<Board>(Id.For<Board>(userId));

        if (!entity.EntityExists)
            return new NotFoundResult();

        // Verify board
        var initialState = entity.EntityState.InitialState;
        var gameState = new BinaryData(payload.GameState).ToArray();

        if (!State.IsEquivalent(initialState, gameState))
            return new BadRequestObjectResult(new string[]
            {
                "The 'gameState' does not match the initial board state"
            });

        if (!State.IsCompleted(gameState))
            return new BadRequestObjectResult(new string[]
            {
                "The 'gameState' is not yet completed"
            });

        // Notify orchestrator
        await orchestrationClient.RaiseEventAsync(
            Id.ForInstance(nameof(BoardManagerOrchestratorFunction), lobbyId, userId),
            DurableEvents.BOARD_COMPLETED);

        // Respond to request
        return new NoContentResult();
    }
}
