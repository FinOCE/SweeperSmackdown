﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Models;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
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

        await ctx.CallActivityAsync(
            nameof(LobbyStateSetActivityFunction),
            new LobbyStateSetActivityFunctionProps(lobbyId, ELobbyState.ConfigureUnlocked));

        // Wait for settings to be locked in
        await ctx.WaitForExternalEvent(DurableEvents.GAME_START_LOCK);

        await ctx.CallActivityAsync(
            nameof(LobbyStateSetActivityFunction),
            new LobbyStateSetActivityFunctionProps(lobbyId, ELobbyState.ConfigureLocked));

        // Either unlock changing settings or confirm
        var unlock = ctx.WaitForExternalEvent(DurableEvents.GAME_START_UNLOCK);
        var confirm = ctx.WaitForExternalEvent(DurableEvents.GAME_START_CONFIRMATION);

        var winner = await Task.WhenAny(unlock, confirm);

        // Restart orchestration if unlocking
        if (winner == unlock)
        {
            ctx.ContinueAsNew(null);
            return new GameSettings(); // Empty response to continue as new
        }

        // Fetch lobby settings
        var lobby = await ctx.CallActivityAsync<Lobby>(
            nameof(LobbyFetchActivityFunction),
            new LobbyFetchActivityFunctionProps(lobbyId));

        // Start countdown and notify players
        using var timeoutCts = new CancellationTokenSource();
        var expiry = ctx.CurrentUtcDateTime.AddSeconds(Constants.SETUP_COUNTDOWN_DURATION);

        var notification = ctx.CallActivityAsync(
            nameof(GameStartNotifyCountdownActivityFunction),
            new GameStartNotifyCountdownActivityFunctionProps(lobbyId, expiry));

        var timer = ctx.CreateTimer(expiry, timeoutCts.Token);

        await Task.WhenAll(notification, timer);
        return lobby.Settings;
    }
}
