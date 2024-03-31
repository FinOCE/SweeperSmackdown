using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Websocket;

public static class OnDisconnectFunction
{
    [FunctionName(nameof(OnDisconnectFunction))]
    public static async Task Run(
        [WebPubSubTrigger(PubSubConstants.HUB_NAME, WebPubSubEventType.System, "disconnected")] DisconnectedEventRequest req,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var userId = req.ConnectionContext.UserId;
        
        var lobbyContainer = cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.LOBBY_CONTAINER_NAME);

        // Get lobby user is in
        var lobby = lobbyContainer
            .GetItemLinqQueryable<Lobby>()
            .Where(lobby => lobby.UserIds.Contains(userId))
            .FirstOrDefault();
        
        if (lobby == null)
            return;
        
        // Remove user from userIds list
        await lobbyContainer.PatchItemAsync<Lobby>(lobby.Id, new(lobby.Id), new[]
        {
            PatchOperation.Set($"/userIds", lobby.UserIds.Where(id => id != userId).ToArray())
        });

        var updatedLobby = lobby;
        updatedLobby.UserIds = updatedLobby.UserIds.Where(id => id != userId).ToArray();

        // Notify others in lobby
        await ws.AddAsync(ActionFactory.RemoveUserFromLobby(userId, lobby.Id));
        await ws.AddAsync(ActionFactory.UpdateLobby("SYSTEM", lobby.Id, LobbyResponseDto.FromModel(updatedLobby)));
        await ws.AddAsync(ActionFactory.RemoveUser(userId, lobby.Id));

        // Don't delete everything if users still in lobby
        if ((lobby.UserIds.Length - 1) > 0)
            return;
        
        // Delete lobby
        await lobbyContainer.DeleteItemAsync<Lobby>(lobby.Id, new(lobby.Id));


        // Delete vote
        var voteContainer = cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.VOTE_CONTAINER_NAME);

        try
        {
            await voteContainer.DeleteItemAsync<Vote>(lobby.Id, new(lobby.Id));
        }
        catch (CosmosException) { }

        // Delete orchestrations except board managers
        var orchestrationIds = new string[]
        {
            Id.ForInstance(nameof(LobbyOrchestratorFunction), lobby.Id),
            Id.ForInstance(nameof(GameActiveFunction), lobby.Id),
            Id.ForInstance(nameof(GameCelebrationFunction), lobby.Id),
            Id.ForInstance(nameof(GameCleanupFunction), lobby.Id),
            Id.ForInstance(nameof(GameConfigureFunction), lobby.Id),
            Id.ForInstance(nameof(TimerOrchestratorFunction), lobby.Id)
        };

        foreach (var id in orchestrationIds)
            try
            {
                await orchestrationClient.TerminateAsync(id, "Lobby empty");
            }
            catch (Exception) { }

        // Delete board entity map, entities, and board managers
        var boardContainer = cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.BOARD_CONTAINER_NAME);

        try
        {
            BoardEntityMap boardEntityMap = await boardContainer.ReadItemAsync<BoardEntityMap>(
                lobby.Id,
                new(lobby.Id));

            var tasks = boardEntityMap.BoardIds.Select(id =>
                entityClient.SignalEntityAsync<IBoard>(
                    Id.For<Board>(id),
                    board => board.Delete()));

            await Task.WhenAll(tasks);

            foreach (var id in boardEntityMap.BoardIds)
                try
                {
                    await orchestrationClient.TerminateAsync(
                        Id.ForInstance(nameof(BoardManagerOrchestrationFunction), lobby.Id, id),
                        "Lobby empty");
                }
                catch (Exception) { }

            await boardContainer.DeleteItemAsync<BoardEntityMap>(lobby.Id, new(lobby.Id));
        }
        catch (CosmosException) { }
    }
}
