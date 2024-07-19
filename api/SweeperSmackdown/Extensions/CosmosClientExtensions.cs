using Microsoft.Azure.Cosmos;
using SweeperSmackdown.Assets;
using System.Threading.Tasks;

namespace SweeperSmackdown.Extensions;

public static class CosmosClientExtensions
{
    /// <summary>
    /// Get the database which holds all containers for core sweeper smackdown functionality.
    /// </summary>
    /// <param name="cosmosClient">The CosmosClient</param>
    /// <returns>The sweeper smackdown database</returns>
    public static Database GetSmackdownDatabase(this CosmosClient cosmosClient) =>
        cosmosClient.GetDatabase(DatabaseConstants.DATABASE_NAME);

    /// <summary>
    /// Regenerate database containers. For debug purposes only.
    /// </summary>
    /// <param name="cosmosClient">The CosmosClient</param>
    /// <returns>A task which resolves once all containers have been regenerated</returns>
    public static async Task RegenerateContainers(this CosmosClient cosmosClient)
    {
        // Currently unused. Use this to delete then recreate containers when needed.

        await Task.Delay(1);
    }
}
