using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Models;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyPutFunction
{
    [FunctionName(nameof(LobbyPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}")] HttpRequest req,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? existing,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Lobby> db,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        string lobbyId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Return existing lobby if already exists
        if (existing != null)
            return new OkObjectResult(LobbyResponseDto.FromModel(existing));

        // Create lobby
        var lobby = new Lobby(
            lobbyId,
            requesterId,
            new[] { requesterId },
            new Dictionary<string, int>(),
            new Dictionary<string, int>(),
            new GameSettings(Guid.NewGuid().GetHashCode()));

        await db.AddAsync(lobby);

        // Create vote synchronously (duplicate of VoteCreateActivityFunction - fixes not being available instantly)
        await cosmosClient
            .GetVoteContainer()
            .UpsertItemAsync(
                new Vote(
                    lobby.Id,
                    new Dictionary<string, string[]>()
                    {
                        { "READY", Array.Empty<string>() }
                    },
                    VoteUtils.CalculateRequiredVotes(lobby.UserIds.Length),
                new[] { "READY" }),
                new(lobby.Id));

        // Start orchestrator
        await orchestrationClient.StartNewAsync(
            nameof(LobbyOrchestratorFunction),
            Id.ForInstance(nameof(LobbyOrchestratorFunction), lobbyId));

        // Return created lobby
        return new CreatedResult($"/lobbies/{lobbyId}", LobbyResponseDto.FromModel(lobby));
    }
}
