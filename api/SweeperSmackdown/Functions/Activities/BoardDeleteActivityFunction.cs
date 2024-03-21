using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public static class BoardDeleteActivityFunction
{
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableEntityClient entityClient)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var userId = Id.UserFromInstance(ctx.InstanceId);

        await entityClient.SignalEntityAsync<IBoard>(
            Id.For<Board>(lobbyId, userId),
            board => board.Delete());
    }
}
