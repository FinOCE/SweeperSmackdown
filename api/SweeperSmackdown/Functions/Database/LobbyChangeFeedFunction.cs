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
        [CosmosDB(
            containerName: DatabaseConstants.VOTE_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Vote> voteDb,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        foreach (var lobby in lobbies)
        {
            try
            {
                await ws.AddAsync(ActionFactory.UpdateLobby(lobby.Id, LobbyResponseDto.FromModel(lobby)));

                Vote vote = await cosmosClient
                    .GetVoteContainer()
                    .ReadItemAsync<Vote>(lobby.Id, new(lobby.Id));

                // Handle vote requirement changes and clearing removed member votes
                var requiredVotes = VoteUtils.CalculateRequiredVotes(lobby.UserIds.Length);

                var requiredVotesChanged = requiredVotes != vote.RequiredVotes;
                vote.RequiredVotes = requiredVotes;

                var removedUsers = vote.Votes
                    .SelectMany(kvp => kvp.Value.Where(id => !lobby.UserIds.Contains(id)))
                    .ToArray();

                foreach (var choice in vote.Votes.Keys)
                    vote.Votes[choice] = vote.Votes[choice]
                        .Where(id => lobby.UserIds.Contains(id))
                        .ToArray();

                if (requiredVotesChanged || removedUsers.Length > 0)
                    await voteDb.AddAsync(vote);

                // Add users to websocket and notify
                var unaddedUsers = lobby.UserIds
                    .Where(id => !lobby.AddedUserIds.Contains(id))
                    .ToArray();

                foreach (var id in unaddedUsers)
                {
                    lobby.AddedUserIds = lobby.AddedUserIds.Append(id).Distinct().ToArray();

                    await ws.AddAsync(ActionFactory.AddUserToLobby(id, lobby.Id));
                    await ws.AddAsync(ActionFactory.AddUser(id, lobby.Id));
                }

                // Remove users from websocket and notify
                foreach (var id in removedUsers)
                {
                    lobby.AddedUserIds = lobby.AddedUserIds.Where(userId => userId != id).ToArray();

                    await ws.AddAsync(ActionFactory.RemoveUserFromLobby(id, lobby.Id));
                    await ws.AddAsync(ActionFactory.RemoveUser(id, lobby.Id));
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

                // Delete lobby if empty
                if (lobby.UserIds.Length == 0)
                {
                    var lobbyContainer = cosmosClient.GetLobbyContainer();
                    var voteContainer = cosmosClient.GetVoteContainer();
                    var boardContainer = cosmosClient.GetBoardContainer();

                    await lobbyContainer.DeleteItemAsync<Lobby>(lobby.Id, new(lobby.Id));
                    await voteContainer.DeleteItemAsync<Vote>(lobby.Id, new(lobby.Id));

                    BoardEntityMap boardEntityMap = await boardContainer.ReadItemAsync<BoardEntityMap>(
                        lobby.Id,
                        new(lobby.Id));

                    List<string> orchestrationIds = new()
                    {
                        Id.ForInstance(nameof(LobbyOrchestratorFunction), lobby.Id),
                        Id.ForInstance(nameof(GameActiveFunction), lobby.Id),
                        Id.ForInstance(nameof(GameCelebrationFunction), lobby.Id),
                        Id.ForInstance(nameof(GameCleanupFunction), lobby.Id),
                        Id.ForInstance(nameof(GameConfigureFunction), lobby.Id),
                        Id.ForInstance(nameof(TimerOrchestratorFunction), lobby.Id)
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
                        catch (Exception) { } // TODO: Check if this try..catch is needed

                    await boardContainer.DeleteItemAsync<BoardEntityMap>(lobby.Id, new(lobby.Id));
                }
            }
            catch (Exception)
            {
                // TODO: Check if this ever occurs
            }
        }
    }
}
