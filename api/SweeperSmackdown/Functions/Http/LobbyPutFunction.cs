using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SweeperSmackdown.Functions.Http;

public static class LobbyPutFunction
{
    [FunctionName(nameof(LobbyPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        // Check if lobby exists
        var entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));

        if (entity.EntityExists)
            return new OkObjectResult(new
            {
                lobbyId,
                userIds = entity.EntityState.UserIds,
                wins = entity.EntityState.Wins,
                settings = entity.EntityState.Settings
            });

        // Create lobby
        await entityClient.SignalEntityAsync<ILobby>(
            Id.For<Lobby>(lobbyId),
            lobby => lobby.Create());

        await orchestrationClient.StartNewAsync(
            nameof(LobbyOrchestratorFunction),
            Id.ForInstance(nameof(LobbyOrchestratorFunction), lobbyId));
        
        // Return created/updated lobby        
        return new CreatedResult(
            $"/lobbies/{lobbyId}",
            new
            {
                lobbyId,
                userIds = entity.EntityState.UserIds,
                wins = entity.EntityState.Wins,
                settings = entity.EntityState.Settings
            });
    }
}
