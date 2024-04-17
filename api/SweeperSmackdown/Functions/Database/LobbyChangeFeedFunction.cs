using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Models;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Database;

public static class VoteChangeFeedFunction
{
    [FunctionName(nameof(VoteChangeFeedFunction))]
    public static async Task Run(
        [CosmosDBTrigger(
            databaseName: DatabaseConstants.DATABASE_NAME,
            containerName: DatabaseConstants.LOBBY_CONTAINER_NAME,
            Connection = "CosmosDbConnectionString",
            CreateLeaseContainerIfNotExists = true)]
            Lobby lobby)
    {
        // TODO: Handle settings (and other) updates notifying ws here
    }
}
