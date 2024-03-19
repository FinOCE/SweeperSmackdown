using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
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
        [OrchestrationTrigger] IDurableOrchestrationContext ctx,
        [DurableClient] IDurableEntityClient entityClient)
    {
        var props = ctx.GetInput<GameSetupFunctionProps>();

        // TODO: Figure out how to handle user IDs being present here if the user left
        //       (e.g. when the celebration ends and a new game starts)

        // Setup game conditions
        var presetTask = ctx.WaitForExternalEvent("SetPreset");
        
        var lifetimeTask = ctx.WaitForExternalEvent("SetLifetime");
        var modeTask = ctx.WaitForExternalEvent("SetMode");
        var heightTask = ctx.WaitForExternalEvent("SetHeight");
        var widthTask = ctx.WaitForExternalEvent("SetWidth");
        var manualTask = Task.WhenAll(lifetimeTask, modeTask, heightTask, widthTask);

        // TODO: Setup the above events

        await Task.WhenAny(presetTask, manualTask);

        // Wait for countdown to complete
        await ctx.CallSubOrchestratorAsync(
            nameof(CountdownFunction),
            Id.ForInstance(nameof(CountdownFunction), props.InstanceId),
            new CountdownFunctionProps(props.InstanceId, 10));

        // TODO: Confirm the above restarted suborchestrator doesn't complete task on loop

        // Get current game conditions
        var lobby = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(props.InstanceId));
        
        var userIds = lobby.EntityState.UserIds;
        var lifetime = lobby.EntityState.Lifetime!.Value;
        var mode = lobby.EntityState.Mode!.Value;
        var height = lobby.EntityState.Height!.Value;
        var width = lobby.EntityState.Width!.Value;

        // Create game boards
        var gameState = GameStateFactory.Create(mode, height, width);

        var tasks = userIds.Select(userId =>
            entityClient.SignalEntityAsync<IBoard>(
                Id.For<Board>(props.InstanceId, userId),
                board => board.Create(props.InstanceId, userId, height, width, gameState)));
        
        await Task.WhenAll(tasks);

        // Start the game
        ctx.StartNewOrchestration(
            nameof(GameActiveFunction),
            new GameActiveFunctionProps(props.InstanceId, userIds, lifetime, mode, height, width),
            Id.ForInstance(nameof(GameActiveFunction), props.InstanceId));
    }
}
