using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class BoardCreatedActivityFunctionProps
{
    public string LobbyId { get; }

    public string UserId { get; }

    public BoardCreatedActivityFunctionProps(string lobbyId, string userId)
    {
        LobbyId = lobbyId;
        UserId = userId;
    }
}

public static class BoardCreatedActivityFunction
{
    [FunctionName(nameof(BoardCreatedActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> actions)
    {
        var props = ctx.GetInput<BoardCreatedActivityFunctionProps>();
        await actions.AddAsync(ActionFactory.CreateBoard(props.UserId, props.LobbyId));
    }
}
