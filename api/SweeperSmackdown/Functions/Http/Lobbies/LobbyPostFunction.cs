using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Models;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyPostFunction
{
    [FunctionName(nameof(LobbyPostFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "lobbies")] HttpRequest req,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            SqlQuery = "SELECT * FROM c",
            Connection = "CosmosDbConnectionString")]
            IEnumerable<Lobby> lobbies,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Lobby> lobbyDb,
        [CosmosDB(
            containerName: DatabaseConstants.PLAYER_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Player> playerDb,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId is null)
            return new StatusCodeResult(401);

        // Generate ID that doesn't exist yet
        var lobbyId = "";
        Random random = new();

        do
        {
            lobbyId = random.Next(1, 100_000).ToString();
        }
        while (lobbies.Select(l => l.Id).Contains(lobbyId));

        // Create lobby
        var lobby = new Lobby(
            lobbyId,
            requesterId,
            new GameSettings(Guid.NewGuid().GetHashCode()));

        await lobbyDb.AddAsync(lobby);

        // Create host player
        var player = new Player(requesterId, lobbyId, true, 0, 0);

        await playerDb.AddAsync(player);

        await ws.AddAsync(ActionFactory.AddUser(requesterId, lobbyId));
        await ws.AddAsync(ActionFactory.AddUserToLobby(requesterId, lobbyId));

        // Start orchestrator
        await orchestrationClient.StartNewAsync(
            nameof(LobbyOrchestratorFunction),
            Id.ForInstance(nameof(LobbyOrchestratorFunction), lobbyId));

        // Return created lobby
        return new CreatedResult(
            $"/lobbies/{lobbyId}",
            LobbyResponseDto.FromModel(lobby, new[] { player }));
    }
}
