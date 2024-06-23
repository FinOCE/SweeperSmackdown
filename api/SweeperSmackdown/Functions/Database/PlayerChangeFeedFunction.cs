using Microsoft.Azure.Cosmos;
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
        [DurableClient] IDurableOrchestrationClient orchestrationClient)
    {
        foreach (var player in players)
        {
            // Delete lobby if no active players remain
            var competitors = await cosmosClient.GetAllPlayersInLobbyAsync(player.LobbyId);

            if (!competitors.Any(p => p.Active))
                await orchestrationClient.StartNewAsync(
                    nameof(LobbyDeleteOrchestratorFunction),
                    Id.ForInstance(nameof(LobbyDeleteOrchestratorFunction), player.LobbyId));
        }
    }
}
