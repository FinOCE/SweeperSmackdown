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
        [OrchestrationTrigger] IDurableOrchestrationContext ctx,
        [DurableClient] IDurableEntityClient entityClient)
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
        var lobby = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(props.InstanceId));
        
        if (!lobby.EntityExists)
            return;

        var tasks = lobby.EntityState.UserIds.Select(userId =>
            entityClient.SignalEntityAsync<IBoard>(
                Id.For<Board>(props.InstanceId, userId),
                board => board.Delete()));

        await Task.WhenAll(tasks);

        // Start new game
        ctx.StartNewOrchestration(
            nameof(GameSetupFunction),
            new GameSetupFunctionProps(props.InstanceId, props.UserIds),
            Id.ForInstance(nameof(GameSetupFunction), props.InstanceId));
    }
}
