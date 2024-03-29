using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Models;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class VoteDeleteActivityFunctionProps
{
    public string LobbyId { get; }

    public VoteDeleteActivityFunctionProps(string lobbyId)
    {
        LobbyId = lobbyId;
    }
}

public static class VoteDeleteActivityFunction
{
    [FunctionName(nameof(VoteDeleteActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient)
    {
        var props = ctx.GetInput<VoteDeleteActivityFunctionProps>();

        var container = cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.VOTE_CONTAINER_NAME);

        await container.DeleteItemAsync<BoardEntityMap>(props.LobbyId, new(props.LobbyId));
    }
}
