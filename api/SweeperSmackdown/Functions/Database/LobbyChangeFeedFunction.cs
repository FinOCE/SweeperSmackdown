using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Database;

public static class LobbyChangeFeedFunction
{
    [FunctionName(nameof(LobbyChangeFeedFunction))]
    public static async Task Run(
        [CosmosDBTrigger(
            databaseName: DatabaseConstants.DATABASE_NAME,
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            Connection = "CosmosDbConnectionString",
            CreateLeaseContainerIfNotExists = true)]
            IEnumerable<Lobby> lobbies,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        foreach (var lobby in lobbies)
        {
            try
            {
                var initialLobby = lobby;
                
                await ws.AddAsync(ActionFactory.UpdateLobby(lobby.Id, LobbyResponseDto.FromModel(lobby)));

                var unaddedUsers = lobby.UserIds
                    .Where(id => !lobby.AddedUserIds.Contains(id))
                    .ToArray();

                var removedUsers = lobby.AddedUserIds
                    .Where(id => !lobby.UserIds.Contains(id))
                    .ToArray();

                // Add users to websocket and notify
                foreach (var id in unaddedUsers)
                {
                    lobby.AddedUserIds = lobby.AddedUserIds.Append(id).Distinct().ToArray();

                    await ws.AddAsync(ActionFactory.AddUser(id, lobby.Id));
                    await ws.AddAsync(ActionFactory.AddUserToLobby(id, lobby.Id));
                }

                // Remove users from websocket and notify
                foreach (var id in removedUsers)
                {
                    lobby.AddedUserIds = lobby.AddedUserIds.Where(userId => userId != id).ToArray();

                    await ws.AddAsync(ActionFactory.RemoveUser(id, lobby.Id));
                    await ws.AddAsync(ActionFactory.RemoveUserFromLobby(id, lobby.Id));
                }

                // Create boards for players joining mid-game
                if (lobby.State == ELobbyState.Play)
                {
                    foreach (var id in unaddedUsers)
                    {
                        var boardManagerStatus = await orchestrationClient.GetStatusAsync(
                            Id.ForInstance(nameof(BoardManagerOrchestrationFunction), lobby.Id, id));

                        if (boardManagerStatus.IsInactive())
                            await orchestrationClient.StartNewAsync(
                                nameof(BoardManagerOrchestrationFunction),
                                Id.ForInstance(nameof(BoardManagerOrchestrationFunction), lobby.Id, id),
                                new BoardManagerOrchestrationFunctionProps(lobby.Settings));
                    }
                }

                // Update user ID lists
                if (unaddedUsers.Length > 0 || removedUsers.Length > 0)
                {
                    if (!lobby.UserIds.Contains(lobby.HostId) && lobby.UserIds.Length > 0)
                        lobby.HostId = lobby.UserIds.First();

                    await cosmosClient.GetLobbyContainer().PatchItemAsync<Lobby>(lobby.Id, new(lobby.Id), new[]
                    {
                        PatchOperation.Set("/hostId", lobby.HostId),
                        PatchOperation.Set("/userIds", lobby.UserIds),
                        PatchOperation.Set("/addedUserIds", lobby.AddedUserIds)
                    });
                }

                // Delete lobby if empty
                if (lobby.UserIds.Length == 0)
                {
                    var lobbyContainer = cosmosClient.GetLobbyContainer();
                    var boardContainer = cosmosClient.GetBoardContainer();

                    await lobbyContainer.DeleteItemAsync<Lobby>(lobby.Id, new(lobby.Id));

                    BoardEntityMap boardEntityMap = await boardContainer.ReadItemAsync<BoardEntityMap>(
                        lobby.Id,
                        new(lobby.Id));

                    List<string> orchestrationIds = new()
                    {
                        Id.ForInstance(nameof(LobbyOrchestratorFunction), lobby.Id),
                        Id.ForInstance(nameof(GameActiveFunction), lobby.Id),
                        Id.ForInstance(nameof(GameCelebrationFunction), lobby.Id),
                        Id.ForInstance(nameof(GameCleanupFunction), lobby.Id),
                        Id.ForInstance(nameof(GameConfigureFunction), lobby.Id)
                    };

                    orchestrationIds.AddRange(boardEntityMap.BoardIds.Select(id =>
                        Id.ForInstance(nameof(BoardManagerOrchestrationFunction), lobby.Id, id)));

                    var tasks = boardEntityMap.BoardIds.Select(id =>
                        entityClient.SignalEntityAsync<IBoard>(
                            Id.For<Board>(id),
                            board => board.Delete()));

                    await Task.WhenAll(tasks);

                    foreach (var id in orchestrationIds)
                        try
                        {
                            await orchestrationClient.TerminateAsync(
                                Id.ForInstance(nameof(BoardManagerOrchestrationFunction), lobby.Id, id),
                                "Lobby empty");
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Failed to terminate a board manager orchestration (DOES THIS EVER OCCUR?)");
                        }

                    await boardContainer.DeleteItemAsync<BoardEntityMap>(lobby.Id, new(lobby.Id));
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong updating a lobby (DOES THIS EVEN OCCUR?)");
            }
        }
    }
}
