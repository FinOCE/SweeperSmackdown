using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public static class OldLobbyOrchestratorFunction
{
    [FunctionName(nameof(OldLobbyOrchestratorFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);

        // Configure game
        ctx.SetCustomStatus(new LobbyOrchestratorStatus(ELobbyState.Configure));

        var settings = await ctx.CallSubOrchestratorAsync<GameSettings>(
            nameof(GameConfigureFunction),
            Id.ForInstance(nameof(GameConfigureFunction), lobbyId),
            null);

        // Begin play
        ctx.SetCustomStatus(new LobbyOrchestratorStatus(ELobbyState.Play));

        var winnerId = await ctx.CallSubOrchestratorAsync<string?>(
            nameof(GameActiveFunction),
            Id.ForInstance(nameof(GameActiveFunction), lobbyId),
            new GameActiveFunctionProps(settings));

        // Clean up after games
        ctx.SetCustomStatus(new LobbyOrchestratorStatus(ELobbyState.Won));

        await ctx.CallSubOrchestratorAsync(
            nameof(GameCleanupFunction),
            Id.ForInstance(nameof(GameCleanupFunction), lobbyId),
            new GameCleanupFunctionProps(winnerId));

        // Celebrate
        ctx.SetCustomStatus(new LobbyOrchestratorStatus(ELobbyState.Celebrate));

        await ctx.CallSubOrchestratorAsync(
            nameof(GameCelebrationFunction),
            Id.ForInstance(nameof(GameCelebrationFunction), lobbyId),
            new GameCelebrationFunctionProps(winnerId));

        // Restart lobby
        ctx.ContinueAsNew(null);
    }
}

public class LobbyOrchestratorStatus
{
    public ELobbyState State { get; set; }

    public LobbyOrchestratorStatus(ELobbyState state)
    {
        State = state;
    }
}

public enum ELobbyState
{
    Configure,
    Play,
    Won,
    Celebrate
}
