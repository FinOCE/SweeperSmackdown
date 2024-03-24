using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Models;
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
        [CosmosDB(Connection = "%CosmosDbConnectionString%")] CosmosClient cosmosClient)
    {
        var props = ctx.GetInput<BoardDeleteAllActivityFunctionProps>();

        var container = cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.BOARD_CONTAINER_NAME);
        
        BoardEntityMap boardEntityMap = await container.ReadItemAsync<BoardEntityMap>(
            props.LobbyId,
            new(props.LobbyId));

        var tasks = boardEntityMap.BoardIds.Select(boardId =>
            entityClient.SignalEntityAsync<IBoard>(
                Id.For<Board>(boardId),
                board => board.Delete()));

        await Task.WhenAll(tasks);

        await container.DeleteItemAsync<BoardEntityMap>(props.LobbyId, new(props.LobbyId));
    }
}
