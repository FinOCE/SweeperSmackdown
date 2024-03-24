using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public static class UserPutFunction
{
    [FunctionName(nameof(UserPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}/users/{userId}")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId,
        string userId,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> actions)
    {
        var entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));

        // Return 404 if lobby doesn't exist
        if (!entity.EntityExists)
            return new NotFoundResult();

        // Add user to ws group and notify
        await actions.AddAsync(ActionFactory.AddUserToLobby(userId, lobbyId));
        await actions.AddAsync(ActionFactory.AddUser(userId, lobbyId));

        // Return 200 if user already in lobby (probably reconnecting)
        if (entity.EntityState.UserIds.Contains(userId))
            return new OkObjectResult(new UserResponseDto(lobbyId, userId));

        // Add to lobby
        await entityClient.SignalEntityAsync<ILobby>(
            Id.For<Lobby>(lobbyId),
            lobby => lobby.AddUser(userId));

        // Start new board manager if lobby in play
        var status = await orchestrationClient.GetStatusAsync(
            Id.ForInstance(nameof(BoardManagerOrchestrationFunction), lobbyId));

        if (status != null && Enum.Parse<ELobbyOrchestratorFunctionStatus>(status.CustomStatus.ToString()) == ELobbyOrchestratorFunctionStatus.Play)
        {
            await orchestrationClient.StartNewAsync(
                nameof(BoardManagerOrchestrationFunction),
                Id.ForInstance(nameof(BoardManagerOrchestrationFunction), lobbyId, userId),
                new BoardManagerOrchestrationFunctionProps(entity.EntityState.Settings));
        }

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

        // Return created result
        return new CreatedResult($"/lobbies/{lobbyId}/users/{userId}", new UserResponseDto(lobbyId, userId));
    }
}
