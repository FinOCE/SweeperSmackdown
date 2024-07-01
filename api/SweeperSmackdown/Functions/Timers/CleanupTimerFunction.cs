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
        var players = await cosmosClient.GetPlayerContainer()
            .GetItemLinqQueryable<Player>()
            .ToFeedIterator()
            .ReadAllAsync();

        await Task.WhenAll(
            players
                .GroupBy(p => p.LobbyId)
                .Where(g => g.All(p => p.UpdatedAt > 3600))
                .Select(g => orchestrationClient
                    .StartNewAsync(
                        nameof(LobbyDeleteOrchestratorFunction),
                        Id.ForInstance(nameof(LobbyDeleteOrchestratorFunction), g.Key))));
    }
}
