using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Debug;

public static class PurgeFunction
{
#if DEBUG
    [FunctionName(nameof(PurgeFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "debug/purge")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient)
    {
        var database = cosmosClient.GetSmackdownDatabase();

        // Delete all orchestrations
        var orchestrationCts = new CancellationTokenSource();
        var orchestrations = await orchestrationClient.ListInstancesAsync(
            new() { PageSize = 1000 },
            orchestrationCts.Token);

        await Task.WhenAll(
            orchestrations.DurableOrchestrationState.Select(orchestration =>
                Task.Run(async () =>
                {
                    try
                    {
                        await orchestrationClient.TerminateAsync(orchestration.InstanceId, "Purge");
                    }
                    catch (Exception)
                    {
                    }
                
                    await orchestrationClient.PurgeInstanceHistoryAsync(orchestration.InstanceId);
                })));

        // Delete all entities
        var entityCts = new CancellationTokenSource();
        var entities = await entityClient.ListEntitiesAsync(
            new() { PageSize = 1000 },
            entityCts.Token);

        await Task.WhenAll(
            entities.Entities.Select(entity => entityClient
                .SignalEntityAsync(entity.EntityId, "Delete")));

        await entityClient.CleanEntityStorageAsync(true, true, entityCts.Token);

        // Delete all content from database
        await cosmosClient.RegenerateContainers();

        // Respond to request
        return new NoContentResult();
    }
#endif
}
