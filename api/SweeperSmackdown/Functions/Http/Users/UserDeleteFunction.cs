using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Users;

public static class UserDeleteFunction
{
    [FunctionName(nameof(UserDeleteFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "lobbies/{lobbyId}/users/{userId}")] HttpRequest req,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Lobby? lobby,
        [CosmosDB(
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Lobby> lobbyDb,
        [CosmosDB(
            containerName: DatabaseConstants.VOTE_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{lobbyId}",
            PartitionKey = "{lobbyId}")]
            Vote? vote,
        [CosmosDB(
            containerName: DatabaseConstants.VOTE_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Vote> voteDb,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws,
        string lobbyId,
        string userId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Check if lobby exists and that user is in it
        if (lobby == null)
            return new NotFoundResult();

        // Only allow the specific user and the host to delete them
        if (requesterId != userId && lobby.HostId != userId)
            return new StatusCodeResult(403);

        // Remove user from lobby
        lobby.UserIds = lobby.UserIds.Where(id => id != userId).ToArray();
        await lobbyDb.AddAsync(lobby);

        await ws.AddAsync(ActionFactory.RemoveUserFromLobby(userId, lobbyId));
        await ws.AddAsync(ActionFactory.UpdateLobby("SYSTEM", lobbyId, LobbyResponseDto.FromModel(lobby)));
        await ws.AddAsync(ActionFactory.RemoveUser(userId, lobbyId));

        // Update votes required
        var requiredVotes = VoteUtils.CalculateRequiredVotes(lobby.UserIds.Length);

        if (vote != null && vote.RequiredVotes != requiredVotes)
        {
            vote.RequiredVotes = requiredVotes;
            await voteDb.AddAsync(vote);
            await ws.AddAsync(ActionFactory.UpdateVoteState(lobbyId, VoteGroupResponseDto.FromModel(vote)));
        }

        // Respond to request
        return new NoContentResult();
    }
}
