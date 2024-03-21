using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class BoardCreateActivityFunctionProps
{
    public string LobbyId { get; }

    public string UserId { get; }
    
    public byte[] GameState { get; }

    public BoardCreateActivityFunctionProps(string lobbyId, string userId, byte[] gameState)
    {
        LobbyId = lobbyId;
        UserId = userId;
        GameState = gameState;
    }
}

public static class BoardCreateActivityFunction
{
    [FunctionName(nameof(BoardCreateActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableEntityClient entityClient)
    {
        var props = ctx.GetInput<BoardCreateActivityFunctionProps>();

        await entityClient.SignalEntityAsync<IBoard>(
            Id.For<Board>(props.LobbyId, props.UserId),
            board => board.Create(props.GameState));
    }
}
