using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Factories;
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
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var props = ctx.GetInput<WinAddActivityFunctionProps>();

        var container = cosmosClient.GetLobbyContainer();
        
        Lobby lobby = await container.ReadItemAsync<Lobby>(props.LobbyId, new(props.LobbyId));
        var wins = lobby.Wins.ContainsKey(props.WinnerId) ? lobby.Wins[props.WinnerId] + 1 : 1;

        await container.PatchItemAsync<Lobby>(props.LobbyId, new(props.LobbyId), new[]
        {
            PatchOperation.Set($"/wins/{props.WinnerId}", wins),
            PatchOperation.Replace($"/scores", new Dictionary<string, int>())
        });
        lobby.Wins[props.WinnerId] = wins;
        lobby.Scores = new Dictionary<string, int>();

        await ws.AddAsync(ActionFactory.UpdateLobby(props.WinnerId, lobby.Id, LobbyResponseDto.FromModel(lobby)));
    }
}
