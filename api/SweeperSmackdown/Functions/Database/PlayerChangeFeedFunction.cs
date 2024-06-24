using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Database;

public static class PlayerChangeFeedFunction
{
    [FunctionName(nameof(PlayerChangeFeedFunction))]
    public static async Task Run(
        [CosmosDBTrigger(
            databaseName: DatabaseConstants.DATABASE_NAME,
            containerName: DatabaseConstants.PLAYER_CONTAINER_NAME,
            Connection = "CosmosDbConnectionString",
            CreateLeaseContainerIfNotExists = true)]
            IEnumerable<Player> players,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Lobby> lobbyDb,
        [DurableClient] IDurableOrchestrationClient orchestrationClient)
    {
        foreach (var player in players)
        {
            // Update host for lobbies the player was hosting
            var lobbies = await cosmosClient
                .GetLobbyContainer()
                .GetItemLinqQueryable<Lobby>()
                    .Where(l => l.HostId == player.Id)
                .ToFeedIterator()
                .ReadAllAsync();

            await Task.WhenAll(
                lobbies.Select(async lobby =>
                {
                    var participants = await cosmosClient.GetAllPlayersInLobbyAsync(lobby.Id);
                    var newHost = participants.Where(p => p.Active).FirstOrDefault();

                    if (newHost is not null)
                    {
                        lobby.HostId = newHost.Id;
                        await lobbyDb.AddAsync(lobby);
                    }
                }));

            // Delete lobby if no active players remain
            var competitors = await cosmosClient.GetAllPlayersInLobbyAsync(player.LobbyId);

            if (!competitors.Any(p => p.Active))
                await orchestrationClient.StartNewAsync(
                    nameof(LobbyDeleteOrchestratorFunction),
                    Id.ForInstance(nameof(LobbyDeleteOrchestratorFunction), player.LobbyId));
        }
    }
}
