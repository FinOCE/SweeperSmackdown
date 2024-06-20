using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public class GameCelebrationFunctionProps
{
    public string? WinnerId { get; }

    public GameCelebrationFunctionProps(string? winnerId)
    {
        WinnerId = winnerId;
    }
}

public static class GameCelebrationFunction
{
    [FunctionName(nameof(GameCelebrationFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var props = ctx.GetInput<GameCelebrationFunctionProps>();

        // Set state to celebrate
        var expiry = ctx.CurrentUtcDateTime.AddSeconds(Constants.CELEBRATION_COUNTDOWN_DURATION);

        await ctx.CallActivityAsync(
            nameof(LobbyStateSetActivityFunction),
            new LobbyStateSetActivityFunctionProps(lobbyId, ELobbyState.Celebrate, expiry));

        // Start countdown
        using var timeoutCts = new CancellationTokenSource();
        await ctx.CreateTimer(expiry, timeoutCts.Token);
    }
}
