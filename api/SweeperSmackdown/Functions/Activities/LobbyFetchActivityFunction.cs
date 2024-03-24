using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Models;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class LobbyFetchActivityFunctionProps
{
    public string LobbyId { get; }

    public LobbyFetchActivityFunctionProps(string lobbyId)
    {
        LobbyId = lobbyId;
    }
}

public static class LobbyFetchActivityFunction
{
    [FunctionName(nameof(LobbyFetchActivityFunction))]
    public static async Task<Lobby> Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient)
    {
        var props = ctx.GetInput<LobbyFetchActivityFunctionProps>();

        var container = cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.LOBBY_CONTAINER_NAME);

        return await container.ReadItemAsync<Lobby>(props.LobbyId, new(props.LobbyId));
    }
}
