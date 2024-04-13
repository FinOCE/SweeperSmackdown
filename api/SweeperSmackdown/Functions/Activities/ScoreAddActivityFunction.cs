using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Factories;
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
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var props = ctx.GetInput<ScoreAddActivityFunctionProps>();

        var container = cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.LOBBY_CONTAINER_NAME);

        Lobby lobby = await container.ReadItemAsync<Lobby>(props.LobbyId, new(props.LobbyId));
        var wins = lobby.Wins.ContainsKey(props.WinnerId) ? lobby.Wins[props.WinnerId] + 1 : 1;

        await container.PatchItemAsync<Lobby>(props.LobbyId, new(props.LobbyId), new[]
        {
            PatchOperation.Set($"/scores/{props.WinnerId}", wins)
        });

        await ws.AddAsync(ActionFactory.UpdateLobby(props.WinnerId, lobby.Id, LobbyResponseDto.FromModel(lobby)));
    }
}
