﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;

namespace SweeperSmackdown.Functions.Websocket;

public static class OnConnectedFunction
{
    [FunctionName(nameof(OnConnectedFunction))]
    public static void Run(
        [WebPubSubTrigger(PubSubConstants.HUB_NAME, WebPubSubEventType.System, "connected")] ConnectedEventRequest req)
    {
    }
}
