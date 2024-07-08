using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public static class GameConfigureFunction
{
    [FunctionName(nameof(GameConfigureFunction))]
    public static async Task<GameSettings> Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        ctx.SetCustomStatus(new GameConfigureStatus(EGameConfigureStatus.Pending));

        // Wait for game settings to be confirmed
        var confirm = ctx.WaitForExternalEvent(DurableEvents.LOBBY_CONFIRMED);

        // Set lobby state to countdown to share timer expiry
        var expiry = ctx.CurrentUtcDateTime.AddSeconds(Constants.SETUP_COUNTDOWN_DURATION);
        ctx.SetCustomStatus(new GameConfigureStatus(EGameConfigureStatus.Confirmed));

        // Fetch lobby settings
        var settings = await ctx.CallEntityAsync<GameSettings>(
            Id.For<GameSettingsStateMachine>(lobbyId),
            nameof(IGameSettingsStateMachine.GetSettings));

        // Create board manager for each user
        var players = await ctx.CallEntityAsync<IEnumerable<Player>>(
            Id.For<LobbyStateMachine>(lobbyId),
            nameof(ILobbyStateMachine.GetPlayers));

        foreach (var player in players)
            _ = ctx.StartNewOrchestration(
                nameof(BoardManagerOrchestratorFunction),
                new BoardManagerOrchestratorFunctionProps(settings),
                Id.ForInstance(nameof(BoardManagerOrchestratorFunction), lobbyId, player.Id));

        // Start countdown
        using var timeoutCts = new CancellationTokenSource();
        await ctx.CreateTimer(expiry, timeoutCts.Token);

        return settings;
    }
}

public class GameConfigureStatus
{
    public EGameConfigureStatus Status { get; set; }

    public GameConfigureStatus(EGameConfigureStatus status)
    {
        Status = status;
    }
}

public enum EGameConfigureStatus
{
    Pending,
    Confirmed
}