using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Websocket;

public static class OnConnectedFunction
{
    [FunctionName(nameof(OnConnectedFunction))]
    public static async Task Run(
        [WebPubSubTrigger(PubSubConstants.HUB_NAME, WebPubSubEventType.System, "connected")] ConnectedEventRequest req,
        [DurableClient] IDurableEntityClient entityClient)
    {
        var userId = req.ConnectionContext.UserId;

        await entityClient.SignalEntityAsync(
            Id.For<ConnectionReference>(userId),
            nameof(IConnectionReference.Create),
            userId);
    }
}
