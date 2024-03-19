﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Utils;
using System.Linq;

namespace SweeperSmackdown.Functions.Http;

public static class UserGetFunction
{
    [FunctionName(nameof(UserGetFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "lobbies/{lobbyId}/users/{userId}")] HttpRequest req,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId,
        string userId)
    {
        var entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));

        if (!entity.EntityExists)
            return new NotFoundResult();

        if (!entity.EntityState.UserIds.Contains(userId))
            return new NotFoundResult();

        return new OkObjectResult(new { userId, lobbyId });
    }
}
