using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace SweeperSmackdown.Functions.Database;

public static class LobbyChangeFeedFunction
{
    [FunctionName(nameof(LobbyChangeFeedFunction))]
    public static async Task Run(
        [CosmosDBTrigger(
            databaseName: DatabaseConstants.DATABASE_NAME,
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            Connection = "CosmosDbConnectionString",
            CreateLeaseContainerIfNotExists = true)]
            Lobby lobby,
        [CosmosDB(
            containerName: DatabaseConstants.VOTE_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Vote> voteDb,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        await ws.AddAsync(ActionFactory.UpdateLobby(lobby.Id, LobbyResponseDto.FromModel(lobby)));

        Vote vote = await cosmosClient
            .GetVoteContainer()
            .ReadItemAsync<Vote>(lobby.Id, new(lobby.Id));

        // Handle vote requirement changes and clearing removed member votes
        var requiredVotes = VoteUtils.CalculateRequiredVotes(lobby.UserIds.Length);

        var requiredVotesChanged = requiredVotes != vote.RequiredVotes;
        vote.RequiredVotes = requiredVotes;

        var removedUsers = vote.Votes
            .SelectMany(kvp => kvp.Value.Where(id => !lobby.UserIds.Contains(id)))
            .ToArray();

        foreach (var choice in vote.Votes.Keys)
            vote.Votes[choice] = vote.Votes[choice]
                .Where(id => lobby.UserIds.Contains(id))
                .ToArray();

        if (requiredVotesChanged || removedUsers.Length > 0)
            await voteDb.AddAsync(vote);

        // Add users to websocket and notify
        var unaddedUsers = lobby.UserIds
            .Where(id => !lobby.AddedUserIds.Contains(id))
            .ToArray();

        foreach (var id in unaddedUsers)
        {
            lobby.AddedUserIds = lobby.AddedUserIds.Append(id).Distinct().ToArray();

            await ws.AddAsync(ActionFactory.AddUserToLobby(id, lobby.Id));
            await ws.AddAsync(ActionFactory.AddUser(id, lobby.Id));
        }

        // Remove users from websocket and notify
        foreach (var id in removedUsers)
        {
            lobby.AddedUserIds = lobby.AddedUserIds.Where(userId => userId != id).ToArray();

            await ws.AddAsync(ActionFactory.RemoveUserFromLobby(id, lobby.Id));
            await ws.AddAsync(ActionFactory.RemoveUser(id, lobby.Id));
        }

        // TODO: Handle settings (and other) updates notifying ws here

        // TODO: Handle creating boards for players joining mid-game

        // TODO: Delete entire lobby if no users left
    }
}
