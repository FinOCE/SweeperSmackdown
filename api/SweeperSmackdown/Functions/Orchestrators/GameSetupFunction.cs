using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public class GameSetupFunctionProps
{
    public GameSettings Settings { get; }

    public GameSetupFunctionProps(GameSettings settings)
    {
        Settings = settings;
    }
}

public static class GameSetupFunction
{
    [FunctionName(nameof(GameSetupFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var props = ctx.GetInput<GameSetupFunctionProps>();

        var gameState = GameStateFactory.Create(props.Settings); // TODO: Handle inconsistent states being generated

        // Create new board entities
        var userIds = await ctx.CallEntityAsync<string[]>(
            Id.For<Lobby>(lobbyId),
            nameof(Lobby.GetUserIds));

        var tasks = userIds.Select(userId =>
            ctx.CallActivityAsync(
                nameof(BoardCreateActivityFunction),
                new BoardCreateActivityFunctionProps(lobbyId, userId, gameState)));

        await Task.WhenAll(tasks);
    }
}
