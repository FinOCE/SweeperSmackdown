using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Models;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public static class GameConfigureFunction
{
    [FunctionName(nameof(GameConfigureFunction))]
    public static async Task<GameSettings> Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);

        // Get current lobby state
        var lobby = await ctx.CallActivityAsync<Lobby>(
            nameof(LobbyFetchActivityFunction),
            new LobbyFetchActivityFunctionProps(lobbyId));

        // Create vote
        await ctx.CallActivityAsync(
            nameof(VoteCreateActivityFunction),
            new VoteCreateActivityFunctionProps(lobby));

        // Wait for countdown to complete
        await ctx.CallSubOrchestratorAsync(
            nameof(TimerOrchestratorFunction),
            Id.ForInstance(nameof(TimerOrchestratorFunction), lobbyId),
            new TimerOrchestratorFunctionProps());

        // Delete vote
        await ctx.CallActivityAsync(
            nameof(VoteDeleteActivityFunction),
            new VoteDeleteActivityFunctionProps(lobbyId));

        // Get updated lobby state (contains configured settings)
        lobby = await ctx.CallActivityAsync<Lobby>(
            nameof(LobbyFetchActivityFunction),
            new LobbyFetchActivityFunctionProps(lobbyId));

        // Return settings to use
        return lobby.Settings;
    }
}
