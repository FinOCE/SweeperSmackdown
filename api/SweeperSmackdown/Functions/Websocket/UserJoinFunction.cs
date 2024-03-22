using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebPubSub.Common;
using SweeperSmackdown.Assets;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Websocket;

public class UserJoinFunctionProps
{
    [JsonPropertyName("instanceId")]
    public string InstanceId { get; } = null!;
}

// TODO: Change this to something else - we don't need to handle USER_JOIN like this because it's sent over HTTP

public static class UserJoinFunction
{
    [FunctionName(nameof(UserJoinFunction))]
    public static async Task<UserEventResponse> Run(
        [WebPubSubTrigger(PubSubConstants.HUB_NAME, WebPubSubEventType.User, PubSubEvents.USER_JOIN)] UserEventRequest req,
        BinaryData data,
        WebPubSubDataType dataType,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> actions)
    {
        // A
        var props = data.ToObjectFromJson<UserJoinFunctionProps>();
        
        // Forward event to other users and return ack
        await actions.AddAsync(
            WebPubSubAction.CreateSendToAllAction(
                BinaryData.FromString($"[{req.ConnectionContext.UserId}] {data}"),
                dataType));
        
        return new UserEventResponse
        {
            Data = BinaryData.FromString("[SYSTEM] ack"),
            DataType = WebPubSubDataType.Text
        };
    }
}
