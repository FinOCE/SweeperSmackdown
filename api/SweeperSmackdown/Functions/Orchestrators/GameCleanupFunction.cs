using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Utils;
using System.Linq;
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
    [FunctionName(nameof(GameConfigureFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var props = ctx.GetInput<GameCleanupFunctionProps>();

        // Delete all board entities
        var userIds = await ctx.CallEntityAsync<string[]>(
            Id.For<Lobby>(lobbyId),
            nameof(Lobby.GetUserIds));

        var tasks = userIds.Select(userId =>
            ctx.CallActivityAsync(
                Id.ForInstance(nameof(BoardDeleteActivityFunction), lobbyId, userId),
                null));

        await Task.WhenAll(tasks);

        // Add winner
        if (props.WinnerId != null)
            await ctx.CallEntityAsync(
                Id.For<Lobby>(lobbyId),
                nameof(Lobby.AddWin),
                props.WinnerId);
    }
}
