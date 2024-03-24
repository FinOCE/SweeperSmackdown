using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class BoardCreatedActivityFunctionProps
{
    public string LobbyId { get; }

    public string UserId { get; }

    public BoardCreatedActivityFunctionProps(string lobbyId, string userId)
    {
        LobbyId = lobbyId;
        UserId = userId;
    }
}

public static class BoardCreatedActivityFunction
{
    [FunctionName(nameof(BoardCreatedActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [CosmosDB(Connection = "%CosmosDbConnectionString%")] CosmosClient cosmosClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var props = ctx.GetInput<BoardCreatedActivityFunctionProps>();

        var container = cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.BOARD_CONTAINER_NAME);

        BoardEntityMap boardEntityMap = await container.ReadItemAsync<BoardEntityMap>(
            props.LobbyId,
            new(props.LobbyId));

        if (!boardEntityMap.BoardIds.Contains(props.UserId))
            await container.PatchItemAsync<BoardEntityMap>(props.LobbyId, new(props.LobbyId), new[]
            {
                PatchOperation.Add("/boardIds", props.UserId)
            });
            
        await ws.AddAsync(ActionFactory.CreateBoard(props.UserId, props.LobbyId));
    }
}
