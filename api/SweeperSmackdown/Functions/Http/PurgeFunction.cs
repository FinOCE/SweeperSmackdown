using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public static class PurgeFunction
{
#if DEBUG
    [FunctionName(nameof(PurgeFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "purge")] HttpRequest req,
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

        var orchestrationPurgeTasks = orchestrations.DurableOrchestrationState.Select(orchestration =>
            Task.Run(async () =>
            {
                try
                {
                    await orchestrationClient.TerminateAsync(orchestration.InstanceId, "Purge");
                }
                catch (Exception)
                {
                    // This is ok
                }
                finally
                {
                    await orchestrationClient.PurgeInstanceHistoryAsync(orchestration.InstanceId);
                }
            }));

        await Task.WhenAll(orchestrationPurgeTasks);
        Console.WriteLine("Up to 1000 orchestrations cleared");

        // Delete all entities
        var entityCts = new CancellationTokenSource();
        var entities = await entityClient.ListEntitiesAsync(
            new() { PageSize = 1000 },
            entityCts.Token);

        var entityPurgeTasks = entities.Entities.Select(entity =>
            entityClient.SignalEntityAsync(entity.EntityId, "Delete"));

        await Task.WhenAll(entityPurgeTasks);
        await entityClient.CleanEntityStorageAsync(true, true, entityCts.Token);
        Console.WriteLine("Up to 1000 entities cleared");

        // Delete all lobbies
        var lobbyContainer = cosmosClient.GetLobbyContainer();
        await lobbyContainer.DeleteContainerAsync();

        await database.CreateContainerAsync(new()
        {
            Id = DatabaseConstants.LOBBY_CONTAINER_NAME,
            PartitionKeyPath = "/id"
        });

        Console.WriteLine("Lobby container deleted and recreated");

        // Delete all players
        var boardContainer = cosmosClient.GetPlayerContainer();
        await boardContainer.DeleteContainerAsync();

        await database.CreateContainerAsync(new()
        {
            Id = DatabaseConstants.PLAYER_CONTAINER_NAME,
            PartitionKeyPath = "/lobbyId"
        });

        Console.WriteLine("Player container deleted and recreated");

        return new NoContentResult();
    }
#endif
}
