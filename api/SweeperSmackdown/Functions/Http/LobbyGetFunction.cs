﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Models;
using System.Linq;
using SweeperSmackdown.Extensions;

namespace SweeperSmackdown.Functions.Http;

public static class LobbyGetFunction
{
    [FunctionName(nameof(LobbyGetFunction))]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "lobbies/{lobbyId}")] HttpRequest req,
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

        // Check if lobby exists
        if (lobby == null)
            return new NotFoundResult();

        // Check if user is in lobby
        if (!lobby.UserIds.Contains(requesterId))
            return new StatusCodeResult(403);
        
        // Respond to request
        return new OkObjectResult(LobbyResponseDto.FromModel(lobby));
    }
}
