using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Utils;
using SweeperSmackdown.DTOs;

namespace SweeperSmackdown.Functions.Http;

public static class LobbyGetFunction
{
    [FunctionName(nameof(LobbyGetFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "lobbies/{lobbyId}")] HttpRequest req,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        var entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));

        if (!entity.EntityExists)
            return new NotFoundResult();

        return new OkObjectResult(
            new LobbyResponseDto(
                lobbyId,
                entity.EntityState.UserIds,
                entity.EntityState.Wins,
                entity.EntityState.Settings));
    }
}
