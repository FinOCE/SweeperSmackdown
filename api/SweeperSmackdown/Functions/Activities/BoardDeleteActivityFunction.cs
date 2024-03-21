using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class BoardDeleteActivityFunctionProps
{
    public string LobbyId { get; }

    public string UserId { get; }

    public BoardDeleteActivityFunctionProps(string lobbyId, string userId)
    {
        LobbyId = lobbyId;
        UserId = userId;
    }
}

public static class BoardDeleteActivityFunction
{
    [FunctionName(nameof(BoardDeleteActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableEntityClient entityClient)
    {
        var props = ctx.GetInput<BoardDeleteActivityFunctionProps>();

        await entityClient.SignalEntityAsync<IBoard>(
            Id.For<Board>(props.LobbyId, props.UserId),
            board => board.Delete());
    }
}
