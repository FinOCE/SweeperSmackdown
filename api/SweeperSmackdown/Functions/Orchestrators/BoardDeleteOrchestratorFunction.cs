using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public class BoardDeleteOrchestratorFunction
{
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        
        // Delete all board entities
        var userIds = await ctx.CallEntityAsync<string[]>(
            Id.For<Lobby>(lobbyId),
            nameof(Lobby.GetUserIds));

        var tasks = userIds.Select(userId =>
            ctx.CallEntityAsync(
                Id.For<Board>(lobbyId, userId),
                nameof(Board.Delete)));

        await Task.WhenAll(tasks);
    }
}
