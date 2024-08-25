using Microsoft.Azure.Cosmos;
using SweeperSmackdown.Stats.Assets;

namespace SweeperSmackdown.Stats.Extensions;

public static class CosmosClientExtensions
{
    public static Container GetPlayerInfoContainer(this CosmosClient cosmosClient) =>
        cosmosClient.GetContainer(Constants.DATABASE_NAME, Constants.STATS_CONTAINER_NAME);

    public static Container GetAchievementProgressContainer(this CosmosClient cosmosClient) =>
        cosmosClient.GetContainer(Constants.DATABASE_NAME, Constants.ACHIEVEMENTS_CONTAINER_NAME);
}
