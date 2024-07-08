using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // Regenerate seed so the same boards arent generated in subsequent round
        ctx.SignalEntity(
            Id.For<GameSettingsStateMachine>(lobbyId),
            nameof(IGameSettingsStateMachine.RegenerateSeed));

        // Configure game settings
        await ctx.SetStatus(ELobbyStatus.Configuring);
        var settings = await ctx.WaitForExternalEvent<GameSettings>(DurableEvents.LOBBY_CONFIRMED);

        // Start game countdown
        var setupExpiry = ctx.CurrentUtcDateTime.AddSeconds(Constants.SETUP_COUNTDOWN_DURATION);
        await ctx.SetStatus(ELobbyStatus.Starting, setupExpiry);

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
        await ctx.SetStatus(ELobbyStatus.Playing, hasTimeLimit ? playStateExpiry : null);

        var winnerTask = ctx.WaitForExternalEvent<string>(DurableEvents.GAME_WON);
        activeTasks.Add(winnerTask);

        // Determine the winner
        var winner = await Task.WhenAny(activeTasks);
        await ctx.SetStatus(ELobbyStatus.Concluding);

        if (winner == winnerTask)
            limitTimeoutCts.Cancel();

        var winnerId = (winner == winnerTask) ? winnerTask.Result : null;

        // Delete all related boards
        foreach (var player in players)
            ctx.SignalEntity(
                Id.For<Board>(player.Id),
                nameof(IBoard.Delete));

        // Add point to winner if there is one
        if (winnerId != null)
            ctx.SignalEntity(
                Id.For<LobbyStateMachine>(lobbyId),
                nameof(ILobbyStateMachine.AddScore),
                winnerId);

        // Celebrate
        var celebrationExpiry = ctx.CurrentUtcDateTime.AddSeconds(Constants.CELEBRATION_COUNTDOWN_DURATION);
        await ctx.SetStatus(ELobbyStatus.Celebrating, celebrationExpiry);

        using var celebrationTimeoutCts = new CancellationTokenSource();
        await ctx.CreateTimer(celebrationExpiry, celebrationTimeoutCts.Token);

        // Return to configure
        ctx.ContinueAsNew(null);
    }

    private static async Task SetStatus(
        this IDurableOrchestrationContext ctx,
        ELobbyStatus status,
        DateTime? expiry = null)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var orchestratorStatus = new LobbyOrchestratorStatus(status, expiry);

        ctx.SetCustomStatus(orchestratorStatus);

        await ctx.CallActivityAsync(
            nameof(NotifyActivityFunction),
            new NotifyActivityFunctionProps(ActionFactory.UpdateLobbyStatus(lobbyId, orchestratorStatus)));
    }
}

public class LobbyOrchestratorStatus
{
    [JsonProperty("status")]
    public ELobbyStatus Status { get; set; }

    [JsonProperty("statusUntil")]
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
