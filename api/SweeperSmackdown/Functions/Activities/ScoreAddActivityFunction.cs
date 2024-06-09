using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Models;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class ScoreAddActivityFunctionProps
{
    public string LobbyId { get; }

    public string WinnerId { get; }

    public ScoreAddActivityFunctionProps(string lobbyId, string winnerId)
    {
        LobbyId = lobbyId;
        WinnerId = winnerId;
    }
}

public static class ScoreAddActivityFunction
{
    [FunctionName(nameof(ScoreAddActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient)
    {
        var props = ctx.GetInput<ScoreAddActivityFunctionProps>();

        await cosmosClient.GetPlayerContainer().PatchItemAsync<Player>(props.WinnerId, new(props.LobbyId), new[]
        {
            PatchOperation.Increment("/score", 1)
        });
    }
}
