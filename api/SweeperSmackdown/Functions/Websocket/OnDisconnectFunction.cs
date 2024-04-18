using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Websocket;

public static class OnDisconnectFunction
{
    [FunctionName(nameof(OnDisconnectFunction))]
    public static async Task Run(
        [WebPubSubTrigger(PubSubConstants.HUB_NAME, WebPubSubEventType.System, "disconnected")] DisconnectedEventRequest req,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient)
    {
        var userId = req.ConnectionContext.UserId;
        
        var lobbyContainer = cosmosClient.GetLobbyContainer();

        // Get lobby user is in
        var lobby = lobbyContainer
            .GetItemLinqQueryable<Lobby>()
            .Where(lobby => lobby.UserIds.Contains(userId))
            .FirstOrDefault();
        
        if (lobby == null)
            return;
        
        // Remove user from userIds list
        await lobbyContainer.PatchItemAsync<Lobby>(lobby.Id, new(lobby.Id), new[]
        {
            PatchOperation.Set($"/userIds", lobby.UserIds.Where(id => id != userId).ToArray())
        });
    }
}
