using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Models;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class WinAddActivityFunctionProps
{
    public string LobbyId { get; }

    public string WinnerId { get; }

    public WinAddActivityFunctionProps(string lobbyId, string winnerId)
    {
        LobbyId = lobbyId;
        WinnerId = winnerId;
    }
}

public static class WinAddActivityFunction
{
    [FunctionName(nameof(WinAddActivityFunction))]
    public static async Task<Lobby> Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [CosmosDB(Connection = "%CosmosDbConnectionString%")] CosmosClient cosmosClient)
    {
        var props = ctx.GetInput<WinAddActivityFunctionProps>();

        var container = cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.LOBBY_CONTAINER_NAME);

        var lobby = await container.ReadItemAsync<Lobby>(props.LobbyId, new(props.LobbyId));
    }
}
