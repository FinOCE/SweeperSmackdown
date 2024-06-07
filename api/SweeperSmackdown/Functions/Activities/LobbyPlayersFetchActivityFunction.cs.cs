using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class LobbyPlayersFetchActivityFunctionProps
{
    public string LobbyId { get; }

    public LobbyPlayersFetchActivityFunctionProps(string lobbyId)
    {
        LobbyId = lobbyId;
    }
}

public static class LobbyPlayersFetchActivityFunction
{
    [FunctionName(nameof(LobbyPlayersFetchActivityFunction))]
    public static async Task<IEnumerable<Player>> Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient)
    {
        var props = ctx.GetInput<LobbyPlayersFetchActivityFunctionProps>();
        return await cosmosClient.GetAllPlayersInLobbyAsync(props.LobbyId);
    }
}
