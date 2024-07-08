﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators.Interactions;

public class GameSettingsUnlockOrchestratorFunctionProps
{
    public string RequesterId { get; set; }

    public GameSettingsUnlockOrchestratorFunctionProps(string requesterId)
    {
        RequesterId = requesterId;
    }
}

public static class GameSettingsUnlockOrchestratorFunction
{
    [FunctionName(nameof(GameSettingsUnlockOrchestratorFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var input = ctx.GetInput<GameSettingsUnlockOrchestratorFunctionProps>();

        await ctx.CallEntityAsync(
            Id.For<GameSettingsStateMachine>(lobbyId),
            nameof(IGameSettingsStateMachine.Unlock));
    }
}