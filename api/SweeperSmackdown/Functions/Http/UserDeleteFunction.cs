using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Utils;
using System;
using System.Linq;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;

namespace SweeperSmackdown.Functions.Http;

public static class UserDeleteFunction
{
    [FunctionName(nameof(UserDeleteFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "lobbies/{lobbyId}/users/{userId}")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId,
        string userId,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> actions)
    {
        var entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));

        if (!entity.EntityExists || !entity.EntityState.UserIds.Contains(userId))
            return new NotFoundResult();

        await entityClient.SignalEntityAsync<ILobby>(
            Id.For<Lobby>(lobbyId),
            lobby => lobby.RemoveUser(userId));

        await actions.AddAsync(ActionFactory.RemoveUserFromLobby(userId, lobbyId));
        await actions.AddAsync(ActionFactory.RemoveUser(userId, lobbyId));

        // Update votes required
        entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));
        var vote = await entityClient.ReadEntityStateAsync<Vote>(Id.For<Vote>(lobbyId));

        var requiredVotes = (int)Math.Floor(entity.EntityState.UserIds.Length / Constants.SETUP_REQUIRED_VOTE_RATIO);

        if (vote.EntityExists)
        {
            await entityClient.SignalEntityAsync<IVote>(
                Id.For<Vote>(lobbyId),
                vote => vote.SetRequiredVotes((
                    requiredVotes,
                    lobbyId,
                    orchestrationClient)));

            await actions.AddAsync(ActionFactory.UpdateVoteRequirement(userId, lobbyId, requiredVotes));
        }

        // Return no content result
        return new NoContentResult();
    }
}
