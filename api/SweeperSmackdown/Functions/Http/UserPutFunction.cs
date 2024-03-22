﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Orchestrators;
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
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId,
        string userId,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> actions)
    {
        var entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));

        if (!entity.EntityExists)
            return new NotFoundResult();

        if (entity.EntityState.UserIds.Contains(userId))
            return new OkObjectResult(new { userId, lobbyId });

        await entityClient.SignalEntityAsync<ILobby>(
            Id.For<Lobby>(lobbyId),
            lobby => lobby.AddUser(userId));

        await actions.AddAsync(ActionFactory.AddUserToLobby(userId, lobbyId));
        await actions.AddAsync(ActionFactory.AddUser(userId, lobbyId));

        var status = await orchestrationClient.GetStatusAsync(
            Id.ForInstance(nameof(BoardManagerOrchestrationFunction), lobbyId));
        
        if (status.IsInactive())
        {
            await orchestrationClient.StartNewAsync(
                nameof(BoardManagerOrchestrationFunction),
                Id.ForInstance(nameof(BoardManagerOrchestrationFunction), lobbyId, userId),
                new BoardManagerOrchestrationFunctionProps(entity.EntityState.Settings));
        }

        return new CreatedResult($"/lobbies/{lobbyId}/users/{userId}", new { userId, lobbyId });
    }
}
