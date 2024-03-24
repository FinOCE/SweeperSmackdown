using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Models;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Collections.Generic;
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
        var timerTask = ctx.CallSubOrchestratorAsync(
            nameof(TimerOrchestratorFunction),
            Id.ForInstance(nameof(TimerOrchestratorFunction), lobbyId),
            new TimerOrchestratorFunctionProps(props.Settings.TimeLimit, true));

        if (props.Settings.TimeLimit != 0)
            tasks.Add(timerTask);

        // Create board manager for each user
        var lobby = await ctx.CallActivityAsync<Lobby>(
            nameof(LobbyFetchActivityFunction),
            new LobbyFetchActivityFunctionProps(lobbyId));

        await ctx.CallActivityAsync<BoardEntityMap>(
            nameof(BoardEntityMapCreateActivityFunction),
            new BoardEntityMapCreateActivityFunctionProps(lobbyId));
        
        foreach (var userId in lobby.UserIds)
            _ = ctx.CallSubOrchestratorAsync(
                nameof(BoardManagerOrchestrationFunction),
                Id.ForInstance(nameof(BoardManagerOrchestrationFunction), lobbyId),
                new BoardManagerOrchestrationFunctionProps(props.Settings));

        // TODO: Ensure all players have a board created before starting

        // Listen for a winner
        var winnerTask = ctx.WaitForExternalEvent<string>(DurableEvents.GAME_WON);
        tasks.Add(winnerTask);
        
        // Determine the winner
        var winner = await Task.WhenAny(tasks);
        
        return (winner == winnerTask)
            ? winnerTask.Result
            : null;
    }
}
