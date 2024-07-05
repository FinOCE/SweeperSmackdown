using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public class GameCleanupFunctionProps
{
    public string? WinnerId { get; }

    public GameCleanupFunctionProps(string? winnerId)
    {
        WinnerId = winnerId;
    }
}

public static class GameCleanupFunction
{
    [FunctionName(nameof(GameCleanupFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var props = ctx.GetInput<GameCleanupFunctionProps>();

        // Delete all related boards
        await ctx.CallActivityAsync(
            nameof(BoardDeleteAllActivityFunction),
            new BoardDeleteAllActivityFunctionProps(lobbyId));

        // Add winner
        if (props.WinnerId != null)
            await ctx.CallActivityAsync(
                nameof(WinAddActivityFunction),
                new WinAddActivityFunctionProps(lobbyId, props.WinnerId));

        // Regenerate seed so the same boards arent generated in next round
        await ctx.CallActivityAsync(
            nameof(LobbyRegenerateSeedActivityFunction),
            new LobbyRegenerateSeedActivityFunctionProps(lobbyId));
    }
}
