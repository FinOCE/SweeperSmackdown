﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

[DataContract]
public class GameSetupFunctionProps
{
    [DataMember]
    public string InstanceId { get; }

    [DataMember]
    public string[] UserIds { get; }

    public GameSetupFunctionProps(string instanceId, string[] userIds)
    {
        InstanceId = instanceId;
        UserIds = userIds;
    }
}

public static class GameSetupFunction
{
    [FunctionName(nameof(GameSetupFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var props = ctx.GetInput<GameSetupFunctionProps>();

        // TODO: Figure out how to handle user IDs being present here if the user left
        //       (e.g. when the celebration ends and a new game starts)

        // Setup game conditions
        var lifetimeTask = ctx.WaitForExternalEvent(Events.SET_LIFETIME);
        var modeTask = ctx.WaitForExternalEvent(Events.SET_MODE);
        var heightTask = ctx.WaitForExternalEvent(Events.SET_HEIGHT);
        var widthTask = ctx.WaitForExternalEvent(Events.SET_WIDTH);

        await Task.WhenAll(lifetimeTask, modeTask, heightTask, widthTask);

        // Wait for countdown to complete
        await ctx.CallSubOrchestratorAsync(
            nameof(CountdownFunction),
            Id.ForInstance(nameof(CountdownFunction), props.InstanceId),
            new CountdownFunctionProps(props.InstanceId, 10));

        // TODO: Confirm the above restarted suborchestrator doesn't complete task on loop

        // Get current game conditions
        var lobby = await ctx.CallEntityAsync<Lobby>(
            Id.For<Lobby>(props.InstanceId),
            nameof(Lobby.Get));
        
        var userIds = lobby.UserIds;
        var lifetime = lobby.Lifetime!.Value;
        var mode = lobby.Mode!.Value;
        var height = lobby.Height!.Value;
        var width = lobby.Width!.Value;

        // Create game boards
        var gameState = GameStateFactory.Create(mode, height, width);
        
        var tasks = userIds.Select(userId =>
            ctx.CallEntityAsync(
                Id.For<Board>(props.InstanceId, userId),
                nameof(Board.Create),
                (props.InstanceId, userIds, height, width, gameState)));
        
        await Task.WhenAll(tasks);

        // Start the game
        ctx.StartNewOrchestration(
            nameof(GameActiveFunction),
            new GameActiveFunctionProps(props.InstanceId, userIds, lifetime, mode, height, width),
            Id.ForInstance(nameof(GameActiveFunction), props.InstanceId));
    }
}
