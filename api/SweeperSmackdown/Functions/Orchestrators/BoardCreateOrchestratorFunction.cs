using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public class BoardCreateOrchestratorFunctionProps
{
    public GameSettings Settings { get; }

    public BoardCreateOrchestratorFunctionProps(GameSettings settings)
    {
        Settings = settings;
    }
}

public class BoardCreateOrchestratorFunction
{
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var props = ctx.GetInput<BoardCreateOrchestratorFunctionProps>();
        
        var gameState = GameStateFactory.Create(props.Settings); // TODO: Rethink where this is made

        // Create new board entities
        var userIds = await ctx.CallEntityAsync<string[]>(
            Id.For<Lobby>(lobbyId),
            nameof(Lobby.GetUserIds));

        var tasks = userIds.Select(userId =>
            ctx.CallEntityAsync(
                Id.For<Board>(lobbyId, userId),
                nameof(Board.Create),
                gameState));

        await Task.WhenAll(tasks);
    }
}
