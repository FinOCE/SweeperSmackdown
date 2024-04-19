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

        var container = cosmosClient.GetLobbyContainer();

        Lobby lobby = await container.ReadItemAsync<Lobby>(props.LobbyId, new(props.LobbyId));
        var scores = lobby.Scores.ContainsKey(props.WinnerId) ? lobby.Scores[props.WinnerId] + 1 : 1;

        await container.PatchItemAsync<Lobby>(props.LobbyId, new(props.LobbyId), new[]
        {
            PatchOperation.Set($"/scores/{props.WinnerId}", scores)
        });
    }
}
