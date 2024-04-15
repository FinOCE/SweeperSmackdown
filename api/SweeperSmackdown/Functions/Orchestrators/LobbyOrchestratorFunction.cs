using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public static class LobbyOrchestratorFunction
{
    [FunctionName(nameof(LobbyOrchestratorFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);

        // Configure game
        ctx.SetCustomStatus(ELobbyOrchestratorFunctionStatus.Configure.ToString());
        
        var settings = await ctx.CallSubOrchestratorAsync<GameSettings>(
            nameof(GameConfigureFunction),
            Id.ForInstance(nameof(GameConfigureFunction), lobbyId),
            null);

        // Begin play
        ctx.SetCustomStatus(ELobbyOrchestratorFunctionStatus.Play.ToString());
        
        var winnerId = await ctx.CallSubOrchestratorAsync<string?>(
            nameof(GameActiveFunction),
            Id.ForInstance(nameof(GameActiveFunction), lobbyId),
            new GameActiveFunctionProps(settings));

        Console.WriteLine($"Winner: {winnerId} (LobbyOrchestrator)");

        // Clean up after games
        ctx.SetCustomStatus(ELobbyOrchestratorFunctionStatus.Clean.ToString());
        
        await ctx.CallSubOrchestratorAsync(
            nameof(GameCleanupFunction),
            Id.ForInstance(nameof(GameCleanupFunction), lobbyId),
            new GameCleanupFunctionProps(winnerId));

        // Celebrate
        ctx.SetCustomStatus(ELobbyOrchestratorFunctionStatus.Celebrate.ToString());
        
        await ctx.CallSubOrchestratorAsync(
            nameof(GameCelebrationFunction),
            Id.ForInstance(nameof(GameCelebrationFunction), lobbyId),
            new GameCelebrationFunctionProps(winnerId));

        // Restart lobby
        ctx.ContinueAsNew(null);
    }
}

public enum ELobbyOrchestratorFunctionStatus
{
    Configure,
    Play,
    Clean,
    Celebrate
}
