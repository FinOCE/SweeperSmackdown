using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

[DataContract]
public class GameActiveFunctionProps
{
    [DataMember]
    public string InstanceId { get; }

    [DataMember]
    public string[] UserIds { get; }

    [DataMember]
    public GameSettings Settings { get; }

    public GameActiveFunctionProps(
        string instanceId,
        string[] userIds,
        GameSettings settings)
    {
        InstanceId = instanceId;
        UserIds = userIds;
        Settings = settings;
    }
}

public static class GameActiveFunction
{
    [FunctionName(nameof(GameActiveFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var props = ctx.GetInput<GameActiveFunctionProps>();

        // TODO: Notify users the game has started

        // Wait until a user completes their board or timeout
        using var timeoutCts = new CancellationTokenSource();

        var expiration = ctx.CurrentUtcDateTime.AddSeconds(props.Settings.TimeLimit);
        var timeoutTask = ctx.CreateTimer(expiration, timeoutCts.Token);

        var completedTask = ctx.WaitForExternalEvent<string>("GameCompleted");

        // TODO: Setup the above task

        var winner = await Task.WhenAny(timeoutTask, completedTask);

        if (!timeoutTask.IsCompleted)
            timeoutCts.Cancel();

        // Determine the winner
        string? winnerId = null;
        
        if (winner == completedTask)
            winnerId = completedTask.Result;

        // Start celebration
        ctx.StartNewOrchestration(
            nameof(GameCelebrationFunction),
            new GameCelebrationFunctionProps(props.InstanceId, props.UserIds, winnerId),
            Id.ForInstance(nameof(GameCelebrationFunction), props.InstanceId));
    }
}
