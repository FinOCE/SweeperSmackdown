using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
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
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        foreach (var player in players)
        {
            Lobby lobby = await cosmosClient
                .GetLobbyContainer()
                .ReadItemAsync<Lobby>(player.LobbyId, new(player.LobbyId));

            var participants = await cosmosClient.GetAllPlayersInLobbyAsync(lobby.Id);

            // Notify lobby if player left
            if (!player.Active)
            {
                await ws.AddAsync(ActionFactory.RemoveUser(player.Id, lobby.Id));
                await ws.AddAsync(ActionFactory.RemoveUserFromLobby(player.Id, lobby.Id));
                await ws.AddAsync(ActionFactory.UpdateLobby(
                    lobby.Id,
                    LobbyResponseDto.FromModel(lobby, participants)));
            }

            // Update host for lobbies the player was hosting
            if (!player.Active && lobby.HostId == player.Id)
            {
                var newHost = participants.Where(p => p.Active).FirstOrDefault();

                if (newHost is not null)
                {
                    lobby.HostId = newHost.Id;
                    await lobbyDb.AddAsync(lobby);
                }
            }

            // Delete lobby if no active players remain
            if (participants.All(p => !p.Active))
                await orchestrationClient.StartNewAsync(
                    nameof(LobbyDeleteOrchestratorFunction),
                    Id.ForInstance(nameof(LobbyDeleteOrchestratorFunction), lobby.Id));
        }
    }
}
