using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Models;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies.Users;

public static class LobbyUserDeleteFunction
{
    [FunctionName(nameof(LobbyUserDeleteFunction))]
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
            containerName: DatabaseConstants.PLAYER_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString",
            Id = "{userId}",
            PartitionKey = "{lobbyId}")]
            Player? player,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws,
        string lobbyId,
        string userId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId == null)
            return new StatusCodeResult(401);

        // Check if lobby exists and that user is in it
        if (lobby is null || player is null || !player.Active)
            return new NotFoundResult();

        // Only allow the specific user and the host to delete them
        if (requesterId != userId && lobby.HostId != userId)
            return new StatusCodeResult(403);

        // Remove user from lobby
        await cosmosClient.GetPlayerContainer().PatchItemAsync<Player>(player.Id, new(player.LobbyId), new[]
        {
            PatchOperation.Set("/active", false)
        });

        await ws.AddAsync(ActionFactory.RemoveUser(player.Id, lobby.Id));
        await ws.AddAsync(ActionFactory.RemoveUserFromLobby(player.Id, lobby.Id));

        // Update host of any lobbies that need to
        await cosmosClient.ChangeHostAsync(userId);

        // Respond to request
        return new NoContentResult();
    }
}
