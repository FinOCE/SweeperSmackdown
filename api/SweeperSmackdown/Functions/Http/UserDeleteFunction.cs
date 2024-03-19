﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SweeperSmackdown.Functions.Http;

public static class UserDeleteFunction
{
    [FunctionName(nameof(UserDeleteFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "lobbies/{lobbyId}/users/{userId}")] HttpRequest req,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId,
        string userId)
    {
        await Task.Delay(0);
        return new NoContentResult();
    }
}
