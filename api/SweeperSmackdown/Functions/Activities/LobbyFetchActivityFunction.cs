using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Extensions;
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

        return await cosmosClient
            .GetLobbyContainer()
            .ReadItemAsync<Lobby>(props.LobbyId, new(props.LobbyId));
    }
}
