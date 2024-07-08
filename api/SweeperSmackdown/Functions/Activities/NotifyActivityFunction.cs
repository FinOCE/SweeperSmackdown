using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class NotifyActivityFunctionProps
{
    public WebPubSubAction Action { get; set; }

    public NotifyActivityFunctionProps(WebPubSubAction action)
    {
        Action = action;
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
        await ws.AddAsync(props.Action);
    }
}
