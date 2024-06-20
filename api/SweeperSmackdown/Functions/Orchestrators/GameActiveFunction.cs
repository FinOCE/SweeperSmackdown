using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Models;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public class GameActiveFunctionProps
{
    public GameSettings Settings { get; }

    public GameActiveFunctionProps(GameSettings settings)
    {
        Settings = settings;
    }
}

public static class GameActiveFunction
{
    [FunctionName(nameof(GameActiveFunction))]
    public static async Task<string?> Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var props = ctx.GetInput<GameActiveFunctionProps>();

        var tasks = new List<Task>();

        // Setup timer if needed
        using var timeoutCts = new CancellationTokenSource();
        var hasTimeLimit = props.Settings.TimeLimit != 0;
        var playStateExpiry = ctx.CurrentUtcDateTime.AddSeconds(props.Settings.TimeLimit);

        if (hasTimeLimit)
            tasks.Add(ctx.CreateTimer(playStateExpiry, timeoutCts.Token));

        // Set state to active
        await ctx.CallActivityAsync(
            nameof(LobbyStateSetActivityFunction),
            new LobbyStateSetActivityFunctionProps(lobbyId, ELobbyState.Play, hasTimeLimit ? playStateExpiry : null));

        // Listen for a winner
        var winnerTask = ctx.WaitForExternalEvent<string>(DurableEvents.GAME_WON);
        tasks.Add(winnerTask);
        
        // Determine the winner
        var winner = await Task.WhenAny(tasks);

        if (winner == winnerTask)
            timeoutCts.Cancel();

        // Update state to won
        await ctx.CallActivityAsync(
            nameof(LobbyStateSetActivityFunction),
            new LobbyStateSetActivityFunctionProps(lobbyId, ELobbyState.Won));
        
        // Return winner ID
        return (winner == winnerTask)
            ? winnerTask.Result
            : null;
    }
}
