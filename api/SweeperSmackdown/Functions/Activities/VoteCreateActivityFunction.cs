using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Models;
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
    public static async Task<Vote> Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [CosmosDB(Connection = "%CosmosDbConnectionString%")] CosmosClient cosmosClient)
    {
        var props = ctx.GetInput<VoteCreateActivityFunctionProps>();

        var container = cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.LOBBY_CONTAINER_NAME);

        var requiredVotes = (int)Math.Floor(props.Lobby.UserIds.Length * Constants.SETUP_REQUIRED_VOTE_RATIO);
        
        return await container.CreateItemAsync(
            new Vote(props.Lobby.Id, new Dictionary<string, string[]>(), requiredVotes, new[] { "READY" }),
            new(props.Lobby.Id));
    }
}
