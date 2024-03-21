using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public static class LobbyOrchestratorFunction
{
    [FunctionName(nameof(LobbyOrchestratorFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);

        // Setup
        var settings = await ctx.CallSubOrchestratorAsync<GameSettings>(
            nameof(GameConfigureFunction),
            Id.ForInstance(nameof(GameConfigureFunction), lobbyId),
            null);

        // Setup game
        await ctx.CallSubOrchestratorAsync(
            nameof(GameSetupFunction),
            Id.ForInstance(nameof(GameSetupFunction), lobbyId),
            new GameSetupFunctionProps(settings));

        // Begin play
        var winnerId = await ctx.CallSubOrchestratorAsync<string?>(
            nameof(GameActiveFunction),
            Id.ForInstance(nameof(GameActiveFunction), lobbyId),
            new GameActiveFunctionProps(settings));

        // Clean up after games
        await ctx.CallSubOrchestratorAsync(
            nameof(GameCleanupFunction),
            Id.ForInstance(nameof(GameCleanupFunction), lobbyId),
            new GameCleanupFunctionProps(winnerId));

        // Celebrate
        await ctx.CallSubOrchestratorAsync(
            nameof(GameCelebrationFunction),
            Id.ForInstance(nameof(GameCelebrationFunction), lobbyId),
            new GameCelebrationFunctionProps(winnerId));

        // Restart lobby
        ctx.ContinueAsNew(null);
    }
}
