using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Models;
using System.Collections.Generic;
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

        var container = cosmosClient.GetLobbyContainer();
        
        Lobby lobby = await container.ReadItemAsync<Lobby>(props.LobbyId, new(props.LobbyId));
        lobby.Wins[props.WinnerId] = lobby.Wins.ContainsKey(props.WinnerId) ? lobby.Wins[props.WinnerId] + 1 : 1;
        lobby.Scores = new Dictionary<string, int>();

        await container.UpsertItemAsync(lobby, new(lobby.Id));
    }
}
