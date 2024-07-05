using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public static class LobbyDeleteOrchestratorFunction
{
    [FunctionName(nameof(LobbyDeleteOrchestratorFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);

        await ctx.CallActivityAsync(
            nameof(LobbyDeleteActivityFunction),
            new LobbyDeleteActivityFunctionProps(lobbyId));
    }
}
