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
            containerName: DatabaseConstants.VOTE_CONTAINER_NAME,
            Connection = "CosmosDbConnectionString",
            CreateLeaseContainerIfNotExists = true)]
            Vote vote)
    {
        // TODO: Handle vote requirement changes
        // TODO: Handle starting/stopping vote here
    }
}
