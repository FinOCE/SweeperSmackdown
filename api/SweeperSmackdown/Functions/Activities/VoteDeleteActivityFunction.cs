using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Extensions;
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

        await cosmosClient
            .GetVoteContainer()
            .DeleteItemAsync<Vote>(props.LobbyId, new(props.LobbyId));
    }
}
