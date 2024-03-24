using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using System;

namespace SweeperSmackdown.Functions.Websocket;

// TODO: This is temporary, delete once testing events is completed

public static class TestFunction
{
#if DEBUG
    [FunctionName("TEST_ONLY__WebPubSubConnect")]
    public static void Connect(
        [WebPubSubTrigger(PubSubConstants.HUB_NAME, WebPubSubEventType.System, "connect")] ConnectEventRequest req)
    {
        Console.WriteLine("User connecting: " + req.ConnectionContext.UserId);
    }

    [FunctionName("TEST_ONLY__WebPubSubConnected")]
    public static void Connected(
        [WebPubSubTrigger(PubSubConstants.HUB_NAME, WebPubSubEventType.System, "connected")] ConnectedEventRequest req)
    {
        Console.WriteLine("User connected: " + req.ConnectionContext.UserId);
    }

    [FunctionName("TEST_ONLY__WebPubSubDisconnected")]
    public static void Disconnected(
        [WebPubSubTrigger(PubSubConstants.HUB_NAME, WebPubSubEventType.System, "disconnected")] DisconnectedEventRequest req)
    {
        Console.WriteLine("User disconnected: " + req.ConnectionContext.UserId);
    }

    [FunctionName("TEST_ONLY__HttpEventHandler")]
    public static IActionResult Http(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "eventhandler")] HttpRequest req)
    {
        Console.WriteLine("Hit test event handler");
        return new OkObjectResult("ok");
    }
#endif
}
