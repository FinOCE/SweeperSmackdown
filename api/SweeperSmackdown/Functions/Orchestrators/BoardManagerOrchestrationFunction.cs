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
        Settings = settings;
        Remaining = remaining ?? settings.BoardCount;
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

        // Create repeatable durable safe random
        var iteration = props.Settings.BoardCount - props.Remaining;

        var seed = props.Settings.Seed != 0
            ? props.Settings.Seed + iteration
            : ctx.NewGuid().GetHashCode();
        
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
        var eventTask = ctx.WaitForExternalEvent<string>(DurableEvents.BOARD_COMPLETED);

        if (props.Remaining > 0 || props.Remaining == -1)
            ctx.ContinueAsNew(new BoardManagerOrchestrationFunctionProps(props.Settings, props.Remaining - 1));

        // Return user ID as winner if completed before any other task
        return userId;
    }
}
