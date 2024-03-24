using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Models;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Structures;
using System.Collections.Generic;

namespace SweeperSmackdown.Functions.Http;

public static class LobbyPutFunction
{
    [FunctionName(nameof(LobbyPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}")] HttpRequest _,
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
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        string lobbyId)
    {
        // TODO: Get userId for person that made request
        var requesterId = "userId";
        
        // Return existing lobby if already exists
        if (existing != null)
            return new OkObjectResult(LobbyResponseDto.FromModel(existing));

        // Create lobby
        var lobby = new Lobby(
            lobbyId,
            requesterId,
            new[] { requesterId },
            new Dictionary<string, int>(),
            new GameSettings());
        
        await db.AddAsync(lobby);

        // Start orchestrator
        await orchestrationClient.StartNewAsync(
            nameof(LobbyOrchestratorFunction),
            Id.ForInstance(nameof(LobbyOrchestratorFunction), lobbyId));

        // Return created lobby
        return new CreatedResult($"/lobbies/{lobbyId}", LobbyResponseDto.FromModel(lobby));
    }
}
