using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public class VotePutFunctionPayload
{
    public string Choice { get; } = null!;
}

public static class VotePutFunction
{
    [FunctionName(nameof(VotePutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "/lobbies/{lobbyId}/votes/{userId}")] VotePutFunctionPayload payload,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId,
        string userId)
    {
        // Check if a vote is in progress
        var vote = await entityClient.ReadEntityStateAsync<Vote>(Id.For<Vote>(lobbyId));

        if (!vote.EntityExists)
            return new NotFoundResult();
        
        // Ensure the vote is valid
        if (vote.EntityState.Choices.Contains(payload.Choice))
            return new BadRequestObjectResult(new string[]
            {
                $"The 'choice' is not a valid option ({string.Join(", ", vote.EntityState.Choices)})"
            });

        // Add user vote
        await entityClient.SignalEntityAsync<IVote>(
            Id.For<Vote>(lobbyId),
            vote => vote.AddVote((userId, payload.Choice)));

        // Notify voted item if required votes reached
        vote = await entityClient.ReadEntityStateAsync<Vote>(Id.For<Vote>(lobbyId));

        if (vote.EntityState.Votes[payload.Choice].Length == vote.EntityState.RequiredVotes)
        {
            var lobby = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));

            switch (lobby.EntityState.Status)
            {
                case ELobbyStatus.Setup:
                    await orchestrationClient.RaiseEventAsync(
                        Id.ForInstance(nameof(CountdownFunction), lobbyId),
                        Events.START_COUNTDOWN);
                    break;
            }
        }

        // Get updated results and return to user
        return new CreatedResult($"/lobbies/{lobbyId}/votes", vote.EntityState.Votes);
    }
}
