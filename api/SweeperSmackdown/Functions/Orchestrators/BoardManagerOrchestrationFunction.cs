﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public class BoardManagerOrchestrationFunctionProps
{
    public GameSettings Settings;
    
    public int Remaining;

    public BoardManagerOrchestrationFunctionProps(GameSettings settings, int remaining)
    {
        Settings = settings;
        Remaining = remaining;
    }
}

public static class BoardManagerOrchestrationFunction
{
    [FunctionName(nameof(BoardManagerOrchestrationFunction))]
    public static async Task<string> Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var userId = Id.FromInstance(ctx.InstanceId);
        var props = ctx.GetInput<BoardManagerOrchestrationFunctionProps>();

        var eventTask = ctx.WaitForExternalEvent<string>(DurableEvents.BOARD_COMPLETED);

        var gameState = GameStateFactory.Create(props.Settings); // TODO: Handle consistent states being generated

        await ctx.CallActivityAsync(
            nameof(BoardCreateActivityFunction),
            new BoardCreateActivityFunctionProps(lobbyId, userId, gameState));

        if (props.Remaining > 0 || props.Remaining == -1)
            ctx.ContinueAsNew(new BoardManagerOrchestrationFunctionProps(props.Settings, props.Remaining - 1));

        return userId;
    }
}
