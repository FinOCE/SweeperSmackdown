using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public class UserPutFunctionPayload
{
}

public static class UserPutFunction
{
    [FunctionName(nameof(UserPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}/users/{userId}")] UserPutFunctionPayload payload,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId,
        string userId)
    {
        var entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));

        if (!entity.EntityExists)
            return new NotFoundResult();

        if (entity.EntityState.UserIds.Contains(userId))
            return new OkObjectResult(new { userId, lobbyId });

        await entityClient.SignalEntityAsync<ILobby>(
            Id.For<Lobby>(lobbyId),
            lobby => lobby.AddUser(userId));

        return new CreatedResult($"/lobbies/{lobbyId}/users/{userId}", new { userId, lobbyId });
    }
}
