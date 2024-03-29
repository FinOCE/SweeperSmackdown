using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using System;

namespace SweeperSmackdown.Functions.Websocket;

public static class OnConnectFunction
{
    [FunctionName(nameof(OnConnectFunction))]
    public static void Run(
        [WebPubSubTrigger(PubSubConstants.HUB_NAME, WebPubSubEventType.System, "connect")] ConnectEventRequest req)
    {
        Console.WriteLine("Connect");
    }
}
