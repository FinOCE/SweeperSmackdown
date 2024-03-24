﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Models;

namespace SweeperSmackdown.Functions.Http;

public static class UserDeleteFunction
{
    [FunctionName(nameof(UserDeleteFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "lobbies/{lobbyId}/users/{userId}")] HttpRequest _,
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
        // TODO: Get userId for person that made request
        var requesterId = "userId";
        
        // Check if lobby exists and that user is in it
        if (lobby == null || !lobby.UserIds.Contains(userId))
            return new NotFoundResult();

        // Only allow the specific user and the host to delete them
        if (requesterId != userId && lobby.HostId != userId)
            return new ForbidResult();

        // Remove user from lobby
        lobby.UserIds = lobby.UserIds.Where(id => id != userId).ToArray();
        await lobbyDb.AddAsync(lobby);
        
        await ws.AddAsync(ActionFactory.RemoveUserFromLobby(userId, lobbyId));
        await ws.AddAsync(ActionFactory.RemoveUser(userId, lobbyId));

        // Update votes required
        var requiredVotes = (int)Math.Floor(lobby.UserIds.Length / Constants.SETUP_REQUIRED_VOTE_RATIO);

        if (vote != null && vote.RequiredVotes != requiredVotes)
        {
            vote.RequiredVotes = requiredVotes;
            await voteDb.AddAsync(vote);
            await ws.AddAsync(ActionFactory.UpdateVoteRequirement(userId, lobbyId, requiredVotes));
        }

        // Respond to request
        return new NoContentResult();
    }
}
