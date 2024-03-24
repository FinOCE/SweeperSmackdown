using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Models;
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
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "%CosmosDbConnectionString%",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? lobby,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "%CosmosDbConnectionString%")]
            IAsyncCollector<Lobby> lobbyDb,
        [CosmosDB(
            containerName: DatabaseConstants.VOTE_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "%CosmosDbConnectionString%",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Vote? vote,
        [CosmosDB(
            containerName: DatabaseConstants.VOTE_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "%CosmosDbConnectionString%")]
            IAsyncCollector<Vote> voteDb,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws,
        string lobbyId,
        string userId)
    {
        // TODO: Get userId for person that made request
        var requesterId = "userId";

        // Check if lobby exists and that user is in it
        if (lobby == null || !lobby.UserIds.Contains(userId))
            return new NotFoundResult();

        // Only allow the specific user and the host to delete them
        if (requesterId != userId && lobby.HostId != userId)
            return new ForbidResult();
        
        // Add user to ws group and notify
        await ws.AddAsync(ActionFactory.AddUserToLobby(userId, lobbyId));
        await ws.AddAsync(ActionFactory.AddUser(userId, lobbyId));

        // Return 200 if user already in lobby (probably reconnecting)
        if (lobby.UserIds.Contains(userId))
            return new OkObjectResult(UserResponseDto.FromModel(lobby, userId));

        // Add to lobby
        lobby.UserIds = lobby.UserIds.Append(userId).ToArray();
        await lobbyDb.AddAsync(lobby);

        // Start new board manager if lobby in play
        var orchestrationStatus = await orchestrationClient.GetStatusAsync(
            Id.ForInstance(nameof(LobbyOrchestratorFunction), lobbyId));

        if (orchestrationStatus != null)
        {
            var customStatus = orchestrationStatus.CustomStatus.ToString();
            var status = Enum.Parse<ELobbyOrchestratorFunctionStatus>(customStatus);

            if (status == ELobbyOrchestratorFunctionStatus.Play)
                await orchestrationClient.StartNewAsync(
                    nameof(BoardManagerOrchestrationFunction),
                    Id.ForInstance(nameof(BoardManagerOrchestrationFunction), lobbyId, userId),
                    new BoardManagerOrchestrationFunctionProps(lobby.Settings));
        }

        // Update votes required
        var requiredVotes = (int)Math.Floor(lobby.UserIds.Length / Constants.SETUP_REQUIRED_VOTE_RATIO);

        if (vote != null && vote.RequiredVotes != requiredVotes)
        {
            vote.RequiredVotes = requiredVotes;
            await voteDb.AddAsync(vote);
            await ws.AddAsync(ActionFactory.UpdateVoteRequirement(userId, lobbyId, requiredVotes));
        }

        // Respond to request
        return new CreatedResult(
            $"/lobbies/{lobbyId}/users/{userId}",
            UserResponseDto.FromModel(lobby, userId));
    }
}
