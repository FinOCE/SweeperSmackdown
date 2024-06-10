using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Timers;

public static class CleanupTimerFunction
{
    [FunctionName(nameof(CleanupTimerFunction))]
    public static async Task Run(
        [TimerTrigger("0 0 * * * *")] TimerInfo timer,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient)
    {
        var lobbies = await cosmosClient.GetLobbyContainer()
            .GetItemLinqQueryable<Lobby>()
            .Where(lobby => lobby.UpdatedAt > 3600) // Not changed in past hour (inactive)
            .ToFeedIterator()
            .ReadAllAsync();

        await Task.WhenAll(
            lobbies.Select(lobby => orchestrationClient
                .StartNewAsync(
                    nameof(LobbyDeleteOrchestratorFunction),
                    Id.ForInstance(nameof(LobbyDeleteOrchestratorFunction), lobby.Id))));
    }
}
