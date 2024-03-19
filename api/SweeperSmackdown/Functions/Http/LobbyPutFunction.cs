using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public class LobbyPutFunctionPayload
{
}

public static class LobbyPutFunction
{
    [FunctionName(nameof(LobbyPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}")] LobbyPutFunctionPayload payload,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        var entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));

        if (entity.EntityExists)
            return new OkObjectResult(entity.EntityState);

        await entityClient.SignalEntityAsync<ILobby>(
            Id.For<Lobby>(lobbyId),
            lobby => lobby.Create(lobbyId, Array.Empty<string>()));

        await orchestrationClient.StartNewAsync(nameof(GameSetupFunction), lobbyId);

        entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));
        return new CreatedResult($"/lobbies/{lobbyId}", entity.EntityState);
    }
}
