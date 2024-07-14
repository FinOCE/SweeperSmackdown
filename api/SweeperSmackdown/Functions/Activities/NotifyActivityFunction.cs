using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Newtonsoft.Json;
using SweeperSmackdown.Assets;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class NotifyActivityFunctionProps
{
    public SendToUserAction? SendToUserAction { get; set; }

    public SendToGroupAction? SendToGroupAction { get; set; }

    public SendToConnectionAction? SendToConnectionAction { get; set; }

    public SendToAllAction? SendToAllAction { get; set; }

    [JsonConstructor]
    public NotifyActivityFunctionProps(
        SendToUserAction? sendToUserAction,
        SendToGroupAction? sendToGroupAction,
        SendToConnectionAction? sendToConnectionAction,
        SendToAllAction? sendToAllAction)
    {
        SendToUserAction = sendToUserAction;
        SendToGroupAction = sendToGroupAction;
        SendToConnectionAction = sendToConnectionAction;
        SendToAllAction = sendToAllAction;
    }

    public NotifyActivityFunctionProps(SendToUserAction action)
    {
        SendToUserAction = action;
    }

    public NotifyActivityFunctionProps(SendToGroupAction action)
    {
        SendToGroupAction = action;
    }

    public NotifyActivityFunctionProps(SendToConnectionAction action)
    {
        SendToConnectionAction = action;
    }

    public NotifyActivityFunctionProps(SendToAllAction action)
    {
        SendToAllAction = action;
    }
}

public static class NotifyActivityFunction
{
    [FunctionName(nameof(NotifyActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var props = ctx.GetInput<NotifyActivityFunctionProps>();

        if (props.SendToUserAction is not null)
            await ws.AddAsync(props.SendToUserAction);

        if (props.SendToGroupAction is not null)
            await ws.AddAsync(props.SendToGroupAction);

        if (props.SendToConnectionAction is not null)
            await ws.AddAsync(props.SendToConnectionAction);

        if (props.SendToAllAction is not null)
            await ws.AddAsync(props.SendToAllAction);
    }
}
