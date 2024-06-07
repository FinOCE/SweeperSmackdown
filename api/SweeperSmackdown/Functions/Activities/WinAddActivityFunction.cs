using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Models;
using System.Linq;
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
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient)
    {
        var props = ctx.GetInput<WinAddActivityFunctionProps>();
        var container = cosmosClient.GetPlayerContainer();

        // Add win to winner
        await container.PatchItemAsync<Player>(props.WinnerId, new(props.LobbyId), new[]
        {
            PatchOperation.Increment("/wins", 1)
        });

        // Set all player scores to 0
        var players = await cosmosClient.GetAllPlayersInLobbyAsync(props.LobbyId);

        await Task.WhenAll(
            players.Select(player =>
                container.PatchItemAsync<Player>(player.Id, new(player.LobbyId), new[]
                {
                    PatchOperation.Set("/score", 0)
                })));
    }
}
