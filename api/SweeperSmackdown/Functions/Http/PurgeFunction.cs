﻿using Microsoft.AspNetCore.Http;
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

        // Delete all votes
        var voteContainer = cosmosClient.GetVoteContainer();
        await voteContainer.DeleteContainerAsync();

        await database.CreateContainerAsync(new()
        {
            Id = DatabaseConstants.VOTE_CONTAINER_NAME,
            PartitionKeyPath = "/id"
        });

        Console.WriteLine("Vote container deleted and recreated");

        // Delete all board entity maps
        var boardContainer = cosmosClient.GetBoardContainer();
        await boardContainer.DeleteContainerAsync();

        await database.CreateContainerAsync(new()
        {
            Id = DatabaseConstants.BOARD_CONTAINER_NAME,
            PartitionKeyPath = "/id"
        });

        Console.WriteLine("Board container deleted and recreated");

        // Delete all auth
        var authContainer = cosmosClient.GetAuthContainer();
        await authContainer.DeleteContainerAsync();

        await database.CreateContainerAsync(new()
        {
            Id = DatabaseConstants.AUTH_CONTAINER_NAME,
            PartitionKeyPath = "/id",
            DefaultTimeToLive = 86400 * 14
        });

        Console.WriteLine("Auth container deleted and recreated");

        // Delete all users
        var userContainer = cosmosClient.GetAuthContainer();
        await userContainer.DeleteContainerAsync();

        await database.CreateContainerAsync(new()
        {
            Id = DatabaseConstants.USER_CONTAINER_NAME,
            PartitionKeyPath = "/id"
        });

        Console.WriteLine("User container deleted and recreated");

        // Respond to request
        return new NoContentResult();
    }
#endif
}
