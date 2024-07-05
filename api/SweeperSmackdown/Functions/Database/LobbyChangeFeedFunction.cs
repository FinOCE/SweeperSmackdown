using Microsoft.Azure.Cosmos;
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
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        foreach (var lobby in lobbies)
        {
            var players = await cosmosClient.GetAllPlayersInLobbyAsync(lobby.Id);
            await ws.AddAsync(ActionFactory.UpdateLobby(lobby.Id, LobbyResponseDto.FromModel(lobby, players)));

            // Delete lobby if empty
            if (!players.Any(p => p.Active))
                await orchestrationClient.StartNewAsync(
                    nameof(LobbyDeleteOrchestratorFunction),
                    Id.ForInstance(nameof(LobbyDeleteOrchestratorFunction), lobby.Id));
        }
    }
}
