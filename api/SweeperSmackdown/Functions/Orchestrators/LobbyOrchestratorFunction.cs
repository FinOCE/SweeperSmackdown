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
            nameof(GameSetupFunction),
            Id.ForInstance(nameof(GameSetupFunction), lobbyId),
            null);

        // TODO: Figure out when to create boards

        //var gameState = GameStateFactory.Create(settings);

        //var tasks = lobby.UserIds.Select(userId =>
        //    ctx.CallEntityAsync(
        //        Id.For<Board>(props.InstanceId, userId),
        //        nameof(Board.Create),
        //        (props.InstanceId, userId, lobby.Settings, gameState)));

        //await Task.WhenAll(tasks);

        // Play
        var winnerId = await ctx.CallSubOrchestratorAsync<string?>(
            nameof(GameActiveFunction),
            Id.ForInstance(nameof(GameActiveFunction), lobbyId),
            new GameActiveFunctionProps(settings));

        // Delete game boards
        await ctx.CallSubOrchestratorAsync(
            nameof(BoardDeleteOrchestratorFunction),
            Id.ForInstance(nameof(BoardDeleteOrchestratorFunction), lobbyId),
            null);

        // Celebrate
        await ctx.CallSubOrchestratorAsync(
            nameof(GameCelebrationFunction),
            Id.ForInstance(nameof(GameCelebrationFunction), lobbyId),
            new GameCelebrationFunctionProps(winnerId));
    }
}
