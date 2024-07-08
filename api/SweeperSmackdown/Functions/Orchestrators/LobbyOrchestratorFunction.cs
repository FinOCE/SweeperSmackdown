using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public static class LobbyOrchestratorFunction
{
    [FunctionName(nameof(LobbyOrchestratorFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);

        // Configure game settings
        ctx.SetCustomStatus(new LobbyOrchestratorStatus(ELobbyStatus.Configuring));
        var settings = await ctx.WaitForExternalEvent<GameSettings>(DurableEvents.LOBBY_CONFIRMED);

        // Start game countdown
        var setupExpiry = ctx.CurrentUtcDateTime.AddSeconds(Constants.SETUP_COUNTDOWN_DURATION);
        ctx.SetCustomStatus(new LobbyOrchestratorStatus(ELobbyStatus.Starting, setupExpiry));

        using var setupTimeoutCts = new CancellationTokenSource();
        var timer = ctx.CreateTimer(setupExpiry, setupTimeoutCts.Token);

        // Create board manager for each user
        var players = await ctx.CallEntityAsync<IEnumerable<Player>>(
            Id.For<LobbyStateMachine>(lobbyId),
            nameof(ILobbyStateMachine.GetPlayers));

        foreach (var player in players)
            _ = ctx.StartNewOrchestration(
                nameof(BoardManagerOrchestratorFunction),
                new BoardManagerOrchestratorFunctionProps(settings),
                Id.ForInstance(nameof(BoardManagerOrchestratorFunction), lobbyId, player.Id));

        await timer;

        // Start game timer if configured
        var activeTasks = new List<Task>();

        using var limitTimeoutCts = new CancellationTokenSource();
        var hasTimeLimit = settings.TimeLimit != 0;
        var playStateExpiry = ctx.CurrentUtcDateTime.AddSeconds(settings.TimeLimit);

        if (hasTimeLimit)
            activeTasks.Add(ctx.CreateTimer(playStateExpiry, limitTimeoutCts.Token));

        // Start game
        ctx.SetCustomStatus(new LobbyOrchestratorStatus(ELobbyStatus.Playing, hasTimeLimit ? playStateExpiry : null));

        var winnerTask = ctx.WaitForExternalEvent<string>(DurableEvents.GAME_WON);
        activeTasks.Add(winnerTask);

        // Determine the winner
        var winner = await Task.WhenAny(activeTasks);
        ctx.SetCustomStatus(new LobbyOrchestratorStatus(ELobbyStatus.Concluding, hasTimeLimit ? playStateExpiry : null));

        if (winner == winnerTask)
            limitTimeoutCts.Cancel();

        var winnerId = (winner == winnerTask) ? winnerTask.Result : null;

        // Delete all related boards
        await ctx.CallActivityAsync(
            nameof(BoardDeleteAllActivityFunction),
            new BoardDeleteAllActivityFunctionProps(lobbyId));

        // Add point to winner if there is one
        if (winnerId != null)
            await ctx.CallActivityAsync(
                nameof(WinAddActivityFunction),
                new WinAddActivityFunctionProps(lobbyId, winnerId));

        // Regenerate seed so the same boards arent generated in next round
        await ctx.CallActivityAsync(
            nameof(LobbyRegenerateSeedActivityFunction),
            new LobbyRegenerateSeedActivityFunctionProps(lobbyId));

        // Celebrate
        var celebrationExpiry = ctx.CurrentUtcDateTime.AddSeconds(Constants.CELEBRATION_COUNTDOWN_DURATION);
        ctx.SetCustomStatus(new LobbyOrchestratorStatus(ELobbyStatus.Celebrating, celebrationExpiry));

        using var celebrationTimeoutCts = new CancellationTokenSource();
        await ctx.CreateTimer(celebrationExpiry, celebrationTimeoutCts.Token);

        // Return to configure
        ctx.ContinueAsNew(null);
    }
}

public class LobbyOrchestratorStatus
{
    public ELobbyStatus Status { get; set; }

    public DateTime? StatusUntil { get; set; }

    public LobbyOrchestratorStatus(ELobbyStatus status, DateTime? statusUntil = null)
    {
        Status = status;
        StatusUntil = statusUntil;
    }
}

public enum ELobbyStatus
{
    Configuring,
    Starting,
    Playing,
    Concluding,
    Celebrating
}
