using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Models;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class LobbyRegenerateSeedActivityFunctionProps
{
    public string LobbyId { get; }

    public LobbyRegenerateSeedActivityFunctionProps(string lobbyId)
    {
        LobbyId = lobbyId;
    }
}

public static class LobbyRegenerateSeedActivityFunction
{
    [FunctionName(nameof(LobbyRegenerateSeedActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient)
    {
        var props = ctx.GetInput<LobbyRegenerateSeedActivityFunctionProps>();

        Lobby lobby = await cosmosClient
            .GetLobbyContainer()
            .ReadItemAsync<Lobby>(props.LobbyId, new(props.LobbyId));

        if (lobby.Settings.Seed != 0)
            await cosmosClient.GetLobbyContainer().PatchItemAsync<Lobby>(
                lobby.Id,
                new(lobby.Id),
                new[] { PatchOperation.Set("/seed", Guid.NewGuid().GetHashCode()) });
    }
}
