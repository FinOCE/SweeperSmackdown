﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyPatchFunction
{
    [FunctionName(nameof(LobbyPatchFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "lobbies/{lobbyId}")] LobbyPatchRequestDto payload,
        HttpRequest req,
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
        [CosmosDB(
            containerName: DatabaseConstants.PLAYER_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            SqlQuery = "SELECT * FROM c WHERE c.lobbyId = {lobbyId}",
            Connection = "CosmosDbConnectionString")]
            IEnumerable<Player> players,
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

        if (lobby.State != ELobbyState.ConfigureUnlocked)
            return new ConflictResult();

        // Only allow lobby members to modify
        if (!players.Any(p => p.Id == requesterId))
            return new StatusCodeResult(403);

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

        try
        {
            lobby.Settings = lobby.Settings.Update(
                payload.Mode,
                payload.Height,
                payload.Width,
                payload.Mines,
                payload.Difficulty,
                payload.Lives,
                payload.TimeLimit,
                payload.BoardCount,
                seed);
        }
        catch (ArgumentException)
        {
            return new BadRequestObjectResult(new string[]
            {
                "Unable to set lobby settings due to conflicting values"
            });
        }

        await db.AddAsync(lobby);

        // Respond to request
        return new OkObjectResult(LobbyResponseDto.FromModel(lobby, players));
    }
}
