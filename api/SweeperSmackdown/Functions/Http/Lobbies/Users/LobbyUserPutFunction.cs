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
using SweeperSmackdown.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies.Users;

public static class LobbyUserPutFunction
{
    [FunctionName(nameof(LobbyUserPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}/users/{userId}")] HttpRequest req,
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
            Connection = "CosmosDbConnectionString",
            Id = "{userId}",
            PartitionKey = "{lobbyId}")]
            Player? player,
        [CosmosDB(
            containerName: DatabaseConstants.PLAYER_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            SqlQuery = "SELECT * FROM c WHERE c.lobbyId = {lobbyId}",
            Connection = "CosmosDbConnectionString")]
            IEnumerable<Player> players,
        [CosmosDB(
            containerName: DatabaseConstants.PLAYER_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Player> playerDb,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws,
        string lobbyId,
        string userId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Check if lobby exists and that user is in it
        if (lobby == null)
            return new NotFoundResult();

        // Only allow the specific user join themselves
        if (requesterId != userId)
            return new StatusCodeResult(403);

        // Short circuit if already in lobby
        if (player is not null)
            return new OkObjectResult(LobbyUserResponseDto.FromModel(player));

        // Add to lobby
        player = new Player(userId, lobbyId, true, 0, 0);
        await playerDb.AddAsync(player);

        players = players.Where(p => p.Id != player.Id).Append(player);

        await ws.AddAsync(ActionFactory.AddUser(userId, lobbyId, player));
        await ws.AddAsync(ActionFactory.AddUserToLobby(userId, lobbyId));
        await ws.AddAsync(ActionFactory.UpdateLobby(lobbyId, LobbyResponseDto.FromModel(lobby, players)));

        // Create board for new user if game is in progress
        if (lobby.State == ELobbyState.Play)
        {
            var boardManagerStatus = await orchestrationClient.GetStatusAsync(
                Id.ForInstance(nameof(BoardManagerOrchestratorFunction), lobby.Id, userId));

            if (boardManagerStatus.IsInactive())
                await orchestrationClient.StartNewAsync(
                nameof(BoardManagerOrchestratorFunction),
                    Id.ForInstance(nameof(BoardManagerOrchestratorFunction), lobby.Id, userId),
                    new BoardManagerOrchestratorFunctionProps(lobby.Settings));
        }

        // Respond to request
        return new CreatedResult(
            $"/lobbies/{lobbyId}/users/{userId}",
            LobbyUserResponseDto.FromModel(player));
    }
}
