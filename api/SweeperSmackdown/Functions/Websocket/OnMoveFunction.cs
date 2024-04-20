using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs.Websocket;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Websocket;

public static class OnMoveFunction
{
    [FunctionName(nameof(OnMoveFunction))]
    public static async Task Run(
        [WebPubSubTrigger(PubSubConstants.HUB_NAME, WebPubSubEventType.User, PubSubEvents.MOVE_ADD)] UserEventRequest req,
        [DurableClient] IDurableEntityClient entityClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        // Parse data from request
        var userId = req.ConnectionContext.UserId;
        var data = JsonSerializer.Deserialize<Message<OnMoveData>>(req.Data.ToString())!;

        // Get user board entity
        var entity = await entityClient.ReadEntityStateAsync<Board>(Id.For<Board>(userId));

        if (!entity.EntityExists)
            return;

        var board = entity.EntityState;
            
        // Update board state
        try
        {
            board.MakeMove(data.Data);
            await ws.AddAsync(ActionFactory.MakeMove(data.UserId, data.Data.LobbyId, data.Data));
        }
        catch (InvalidOperationException)
        {
            // Ignore error, prevents event propagation
        }
    }
}
