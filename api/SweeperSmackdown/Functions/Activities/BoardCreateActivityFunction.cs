using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class BoardCreateActivityFunctionProps
{
    public byte[] GameState { get; }

    public BoardCreateActivityFunctionProps(byte[] gameState)
    {
        GameState = gameState;
    }
}

public static class BoardCreateActivityFunction
{
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableEntityClient entityClient)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var userId = Id.UserFromInstance(ctx.InstanceId);
        var props = ctx.GetInput<BoardCreateActivityFunctionProps>();

        await entityClient.SignalEntityAsync<IBoard>(
            Id.For<Board>(lobbyId, userId),
            board => board.Create(props.GameState));
    }
}
