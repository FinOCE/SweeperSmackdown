﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public class BoardManagerOrchestrationFunctionProps
{
    public GameSettings Settings;

    public int Remaining;

    public BoardManagerOrchestrationFunctionProps(GameSettings settings, int? remaining = null)
    {
        int boardCount = settings.BoardCount != 0 ? settings.BoardCount : -1;
        
        Settings = settings;
        Remaining = remaining ?? boardCount;
    }
}

public static class BoardManagerOrchestrationFunction
{
    [FunctionName(nameof(BoardManagerOrchestrationFunction))]
    public static async Task<string> Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var userId = Id.UserFromInstance(ctx.InstanceId);
        var props = ctx.GetInput<BoardManagerOrchestrationFunctionProps>();

        // Create repeatable durable safe random
        var iteration = props.Settings.BoardCount - props.Remaining;

        var seed = props.Settings.Seed != 0
            ? props.Settings.Seed + iteration
            : ctx.NewGuid().GetHashCode();

        Console.WriteLine(lobbyId + " " + userId + " " + seed);

        // Generate boards
        var gameState = GameStateFactory.Create(seed, props.Settings);

        await ctx.CallEntityAsync(
            Id.For<Board>(userId),
            nameof(Board.Create),
            gameState);

        // Notify users the board was created
        await ctx.CallActivityAsync(
            nameof(BoardCreatedActivityFunction),
            new BoardCreatedActivityFunctionProps(lobbyId, userId, gameState));

        // Wait for new board to be completed
        var skippedTask = ctx.WaitForExternalEvent(DurableEvents.BOARD_SKIPPED);
        var completedTask = ctx.WaitForExternalEvent(DurableEvents.BOARD_COMPLETED);

        var winner = await Task.WhenAny(skippedTask, completedTask);
        var decrement = winner == completedTask ? 1 : 0;

        // Start new board if boards still remaining
        props.Remaining = Math.Max(props.Remaining - decrement, -1);
        
        if (props.Remaining > 0 || props.Remaining == -1)
            ctx.ContinueAsNew(
                new BoardManagerOrchestrationFunctionProps(
                    props.Settings,
                    props.Remaining));

        // Return user ID as winner if completed before any other task
        return userId;
    }
}
