using Microsoft.Azure.WebJobs;
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

        // Wait for countdown to complete
        await ctx.CallSubOrchestratorAsync(
            nameof(CountdownFunction),
            Id.ForInstance(nameof(CountdownFunction), props.InstanceId),
            new CountdownFunctionProps(props.InstanceId, Constants.SETUP_COUNTDOWN_DURATION));

        // TODO: Confirm the above restarted suborchestrator doesn't complete task on loop

        // Get current game conditions
        var lobby = await ctx.CallEntityAsync<Lobby>(
            Id.For<Lobby>(props.InstanceId),
            nameof(Lobby.Get));

        // Create game boards
        var gameState = GameStateFactory.Create(lobby.Settings);
        
        var tasks = lobby.UserIds.Select(userId =>
            ctx.CallEntityAsync(
                Id.For<Board>(props.InstanceId, userId),
                nameof(Board.Create),
                (props.InstanceId, userId, lobby.Settings, gameState)));
        
        await Task.WhenAll(tasks);

        // Start the game
        ctx.StartNewOrchestration(
            nameof(GameActiveFunction),
            new GameActiveFunctionProps(props.InstanceId, lobby.UserIds, lobby.Settings),
            Id.ForInstance(nameof(GameActiveFunction), props.InstanceId));
    }
}
