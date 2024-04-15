using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class VoteCreateActivityFunctionProps
{
    public Lobby Lobby { get; }

    public VoteCreateActivityFunctionProps(Lobby lobby)
    {
        Lobby = lobby;
    }
}

public static class VoteCreateActivityFunction
{
    [FunctionName(nameof(VoteCreateActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var props = ctx.GetInput<VoteCreateActivityFunctionProps>();

        var container = cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.VOTE_CONTAINER_NAME);
        
        await container.UpsertItemAsync(
            new Vote(
                props.Lobby.Id,
                new Dictionary<string, string[]>()
                {
                    { "READY", Array.Empty<string>() }
                },
                VoteUtils.CalculateRequiredVotes(props.Lobby.UserIds.Length),
                new[] { "READY" }),
            new(props.Lobby.Id));

        await ws.AddAsync(ActionFactory.StartLobby(props.Lobby.Id));
    }
}
