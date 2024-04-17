using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Models;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class BoardEntityMapCreateActivityFunctionProps
{
    public string LobbyId { get; }

    public BoardEntityMapCreateActivityFunctionProps(string lobbyId)
    {
        LobbyId = lobbyId;
    }
}

public static class BoardEntityMapCreateActivityFunction
{
    [FunctionName(nameof(BoardEntityMapCreateActivityFunction))]
    public static async Task<BoardEntityMap> Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient)
    {
        var props = ctx.GetInput<BoardEntityMapCreateActivityFunctionProps>();

        var container = cosmosClient.GetBoardContainer();

        return await container.UpsertItemAsync(
            new BoardEntityMap(
                props.LobbyId,
                Array.Empty<string>()));
    }
}
