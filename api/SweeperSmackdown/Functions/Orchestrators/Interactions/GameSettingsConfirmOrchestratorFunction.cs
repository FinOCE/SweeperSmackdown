using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators.Interactions;

public class GameSettingsConfirmOrchestratorFunctionProps
{
    public string RequesterId { get; set; }

    public GameSettingsConfirmOrchestratorFunctionProps(string requesterId)
    {
        RequesterId = requesterId;
    }
}

public static class GameSettingsConfirmOrchestratorFunction
{
    [FunctionName(nameof(GameSettingsConfirmOrchestratorFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var input = ctx.GetInput<GameSettingsConfirmOrchestratorFunctionProps>();

        await ctx.CallEntityAsync(
            Id.For<GameSettingsStateMachine>(lobbyId),
            nameof(IGameSettingsStateMachine.Confirm));
    }
}
