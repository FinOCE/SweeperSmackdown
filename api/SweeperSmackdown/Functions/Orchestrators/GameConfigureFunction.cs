using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public static class GameConfigureFunction
{
    [FunctionName(nameof(GameConfigureFunction))]
    public static async Task<GameSettings> Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);

        // Wait for countdown to complete
        await ctx.CallSubOrchestratorAsync(
            nameof(TimerOrchestratorFunction),
            Id.ForInstance(nameof(TimerOrchestratorFunction), lobbyId),
            new TimerOrchestratorFunctionProps(Constants.SETUP_COUNTDOWN_DURATION, false));

        // Get votes necessary to start
        var userIds = await ctx.CallEntityAsync<string[]>(
            Id.For<Lobby>(lobbyId),
            nameof(Lobby.GetUserIds),
            null);

        var requiredVotes = Math.Floor(userIds.Length * Constants.SETUP_REQUIRED_VOTE_RATIO);

        // Create vote
        await ctx.CallEntityAsync(
            Id.For<Vote>(lobbyId),
            nameof(Vote.Create),
            (requiredVotes, new string[] { "READY" }));

        // Get game settings
        var settings = await ctx.CallEntityAsync<GameSettings>(
            Id.For<Lobby>(lobbyId),
            nameof(Lobby.GetSettings));

        return settings;
    }
}
