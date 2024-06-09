using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class BoardDeleteAllActivityFunctionProps
{
    public string LobbyId { get; }

    public BoardDeleteAllActivityFunctionProps(string lobbyId)
    {
        LobbyId = lobbyId;
    }
}

public static class BoardDeleteAllActivityFunction
{
    [FunctionName(nameof(BoardDeleteAllActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableEntityClient entityClient,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient)
    {
        var props = ctx.GetInput<BoardDeleteAllActivityFunctionProps>();

        var players = await cosmosClient.GetAllPlayersInLobbyAsync(props.LobbyId);

        var tasks = players.Select(p =>
            entityClient.SignalEntityAsync<IBoard>(
                Id.For<Board>(p.Id),
                board => board.Delete()));

        await Task.WhenAll(tasks);
    }
}
