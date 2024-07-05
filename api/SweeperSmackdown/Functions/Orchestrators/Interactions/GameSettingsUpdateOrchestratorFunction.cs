using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Functions.Activities.Interactions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators.Interactions;

public class GameSettingsUpdateOrchestratorFunctionProps
{
    public string RequesterId { get; set; }

    public GameSettingsUpdateRequest Updates { get; set; }

    public GameSettingsUpdateOrchestratorFunctionProps(
        string requesterId,
        GameSettingsUpdateRequest updates)
    {
        RequesterId = requesterId;
        Updates = updates;
    }
}

public static class GameSettingsUpdateOrchestratorFunction
{
    [FunctionName(nameof(GameSettingsUpdateOrchestratorFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var input = ctx.GetInput<GameSettingsUpdateOrchestratorFunctionProps>();

        try
        {
            await ctx.CallEntityAsync(
                Id.For<GameSettingsStateMachine>(lobbyId),
                nameof(IGameSettingsStateMachine.UpdateSettings),
                input.Updates);
        }
        catch (Exception)
        {
            await ctx.CallActivityAsync(
                nameof(GameSettingsUpdateSettingsFailedActivityFunction),
                new GameSettingsUpdateSettingsFailedActivityFunctionProps(
                    lobbyId,
                    input.RequesterId,
                    input.Updates));
        }
    }
}
