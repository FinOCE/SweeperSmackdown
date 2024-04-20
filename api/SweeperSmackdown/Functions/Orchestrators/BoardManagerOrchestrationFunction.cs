using Microsoft.Azure.WebJobs;
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
    public static async Task Run(
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

        // Generate boards
        var gameState = GameStateFactory.Create(seed, props.Settings);
        var lives = props.Settings.Lives == 0 ? -1 : props.Settings.Lives;

        await ctx.CallEntityAsync(
            Id.For<Board>(userId),
            nameof(Board.Create),
            (gameState, lives));

        // Notify users the board was created
        await ctx.CallActivityAsync(
            nameof(BoardCreatedActivityFunction),
            new BoardCreatedActivityFunctionProps(lobbyId, userId, gameState));

        // Wait for new board to be completed
        var skippedTask = ctx.WaitForExternalEvent(DurableEvents.BOARD_SKIPPED);
        var completedTask = ctx.WaitForExternalEvent(DurableEvents.BOARD_COMPLETED);
        
        var winner = await Task.WhenAny(skippedTask, completedTask);
        var decrement = winner == completedTask ? 1 : 0;

        // Add score if completed
        if (winner == completedTask)
            await ctx.CallActivityAsync(
                nameof(ScoreAddActivityFunction),
                new ScoreAddActivityFunctionProps(lobbyId, userId));

        // Start new board if boards still remaining or notify orchestrator of completion
        props.Remaining = Math.Max(props.Remaining - decrement, -1);

        if (props.Remaining > 0 || props.Remaining == -1)
        {
            ctx.ContinueAsNew(
                new BoardManagerOrchestrationFunctionProps(
                    props.Settings,
                    props.Remaining));
        }
        else
        {
            await ctx.CallActivityAsync(
                nameof(GameWonActivityFunction),
                new GameWonActivityFunctionProps(lobbyId, userId));
        }
    }
}
