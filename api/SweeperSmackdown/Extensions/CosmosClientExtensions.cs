using Microsoft.Azure.Cosmos;
using SweeperSmackdown.Assets;

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
    /// Get the container holding board entity maps.
    /// </summary>
    /// <param name="cosmosClient">The CosmosClient</param>
    /// <returns>The board container</returns>
    public static Container GetBoardContainer(this CosmosClient cosmosClient) =>
        cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.BOARD_CONTAINER_NAME);
            
    /// <summary>
    /// Get the container holding lobbies.
    /// </summary>
    /// <param name="cosmosClient">The CosmosClient</param>
    /// <returns>The lobby container</returns>
    public static Container GetLobbyContainer(this CosmosClient cosmosClient) =>
        cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.LOBBY_CONTAINER_NAME);
            
    /// <summary>
    /// Get the container holding votes.
    /// </summary>
    /// <param name="cosmosClient">The CosmosClient</param>
    /// <returns>The vote container</returns>
    public static Container GetVoteContainer(this CosmosClient cosmosClient) =>
        cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.VOTE_CONTAINER_NAME);

    /// <summary>
    /// Get the container holding auth.
    /// </summary>
    /// <param name="cosmosClient">The CosmosClient</param>
    /// <returns>The auth container</returns>
    public static Container GetAuthContainer(this CosmosClient cosmosClient) =>
        cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.AUTH_CONTAINER_NAME);

    /// <summary>
    /// Get the container holding users.
    /// </summary>
    /// <param name="cosmosClient">The CosmosClient</param>
    /// <returns>The user container</returns>
    public static Container GetUserContainer(this CosmosClient cosmosClient) =>
        cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.USER_CONTAINER_NAME);
}
