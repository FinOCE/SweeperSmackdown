using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs.Websocket;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Text.Json;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Websocket;

public static class OnMoveFunction
{
    [FunctionName(nameof(OnMoveFunction))]
    public static async Task Run(
        [WebPubSubTrigger(PubSubConstants.HUB_NAME, WebPubSubEventType.User, PubSubEvents.MOVE_ADD)] UserEventRequest req,
        [DurableClient] IDurableEntityClient entityClient)
    {
        // Parse data from request
        var userId = req.ConnectionContext.UserId;
        var data = JsonSerializer.Deserialize<Message<OnMoveData>>(req.Data.ToString())!;
        
        // Update board state
        await entityClient.SignalEntityAsync<IBoard>(
            Id.For<Board>(userId),
            board => board.MakeMove(data.Data));
    }
}
