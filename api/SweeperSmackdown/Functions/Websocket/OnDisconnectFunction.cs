using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Websocket;

public static class OnDisconnectFunction
{
    [FunctionName(nameof(OnDisconnectFunction))]
    public static async Task Run(
        [WebPubSubTrigger(PubSubConstants.HUB_NAME, WebPubSubEventType.System, "disconnected")] DisconnectedEventRequest req,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var userId = req.ConnectionContext.UserId;
        
        var lobbyContainer = cosmosClient.GetLobbyContainer();

        var container = cosmosClient.GetPlayerContainer();

        // Remove any instance of the player from lobbies
        var playerInstances = await cosmosClient
            .GetPlayerContainer()
            .GetItemLinqQueryable<Player>()
                .Where(p => p.Id == userId)
            .ToFeedIterator()
            .ReadAllAsync();

        await Task.WhenAll(
            playerInstances.Select(p => Task.Run(async () => {
                await container.PatchItemAsync<Player>(p.Id, new(p.LobbyId), new[]
                {
                    PatchOperation.Set("/active", false)
                });

                await ws.AddAsync(ActionFactory.RemoveUser(p.Id, p.LobbyId));
                await ws.AddAsync(ActionFactory.RemoveUserFromLobby(p.Id, p.LobbyId));
            })));

        // Update host of any lobbies that need to
        await cosmosClient.ChangeHostAsync(userId);
    }
}
