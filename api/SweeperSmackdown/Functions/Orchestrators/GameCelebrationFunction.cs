using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

[DataContract]
public class GameCelebrationFunctionProps
{
    [DataMember]
    public string InstanceId { get; }

    [DataMember]
    public string[] UserIds { get; }

    [DataMember]
    public string? WinnerId { get; }

    public GameCelebrationFunctionProps(
        string instanceId,
        string[] userIds,
        string? winnerId)
    {
        InstanceId = instanceId;
        UserIds = userIds;
        WinnerId = winnerId;
    }
}

public static class GameCelebrationFunction
{
    [FunctionName(nameof(GameCelebrationFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var props = ctx.GetInput<GameCelebrationFunctionProps>();

        // Wait until celebration end event or timeout
        using var timeoutCts = new CancellationTokenSource();

        var expiration = ctx.CurrentUtcDateTime.AddSeconds(60);
        var timeoutTask = ctx.CreateTimer(expiration, timeoutCts.Token);
        var completedTask = ctx.WaitForExternalEvent("CelebrationEnd");
        await Task.WhenAny(timeoutTask, completedTask);

        if (!timeoutTask.IsCompleted)
            timeoutCts.Cancel();

        // Delete all board entities
        var lobby = await ctx.CallEntityAsync<Lobby>(
            Id.For<Lobby>(props.InstanceId),
            nameof(Lobby.Get));

        var tasks = lobby.UserIds.Select(userId =>
            ctx.CallEntityAsync(
                Id.For<Board>(props.InstanceId, userId),
                nameof(Board.Delete)));

        await Task.WhenAll(tasks);

        // Start new game
        ctx.StartNewOrchestration(
            nameof(GameSetupFunction),
            new GameSetupFunctionProps(props.InstanceId, lobby.UserIds),
            Id.ForInstance(nameof(GameSetupFunction), props.InstanceId));
    }
}
