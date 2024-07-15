﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators;
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
            BoardSolutionRequest payload,
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

        // Check if lobby exists
        var lobby = await entityClient.ReadEntityStateAsync<LobbyStateMachine>(
            Id.For<LobbyStateMachine>(lobbyId));

        if (!lobby.EntityExists)
            return new NotFoundResult();

        // Check if requester is a lobby member and the user specified
        if (!lobby.EntityState.Players.Any(p => p.Id == requesterId) || requesterId != userId)
            return new StatusCodeResult(403);

        // Check if board exists
        var entity = await entityClient.ReadEntityStateAsync<Board>(Id.For<Board>(userId));

        if (!entity.EntityExists)
            return new NotFoundResult();

        if (entity.EntityState.IsDisabled)
            return new ConflictResult();

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
