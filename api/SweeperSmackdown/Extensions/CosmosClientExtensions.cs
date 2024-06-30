using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Get the container holding lobbies.
    /// </summary>
    /// <param name="cosmosClient">The CosmosClient</param>
    /// <returns>The lobby container</returns>
    public static Container GetLobbyContainer(this CosmosClient cosmosClient) =>
        cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.LOBBY_CONTAINER_NAME);

    /// <summary>
    /// Get the container holding players.
    /// </summary>
    /// <param name="cosmosClient">The CosmosClient</param>
    /// <returns>The player container</returns>
    public static Container GetPlayerContainer(this CosmosClient cosmosClient) =>
        cosmosClient.GetContainer(
            DatabaseConstants.DATABASE_NAME,
            DatabaseConstants.PLAYER_CONTAINER_NAME);

    /// <summary>
    /// Get all players in a lobby.
    /// </summary>
    /// <param name="cosmosClient">The CosmosClient</param>
    /// <param name="lobbyId">The ID of the lobby to fetch players in</param>
    /// <returns>An enumerable containing all players associated with the lobby</returns>
    public static async Task<IEnumerable<Player>> GetAllPlayersInLobbyAsync(this CosmosClient cosmosClient, string lobbyId)
    {
        return await cosmosClient
            .GetPlayerContainer()
            .GetItemLinqQueryable<Player>()
                .Where(p => p.LobbyId == lobbyId)
            .ToFeedIterator()
            .ReadAllAsync();
    }

    /// <summary>
    /// Regenerate database containers. For debug purposes only.
    /// </summary>
    /// <param name="cosmosClient">The CosmosClient</param>
    /// <returns>A task which resolves once all containers have been regenerated</returns>
    public static async Task RegenerateContainers(this CosmosClient cosmosClient)
    {
        var database = cosmosClient.GetSmackdownDatabase();

        // Setup lobby container
        try
        {
            await cosmosClient.GetLobbyContainer().DeleteContainerAsync();
        }
        catch (Exception)
        {
        }

        await database.CreateContainerAsync(new()
        {
            Id = DatabaseConstants.LOBBY_CONTAINER_NAME,
            PartitionKeyPath = "/id"
        });

        // Setup player container
        try
        {
            await cosmosClient.GetPlayerContainer().DeleteContainerAsync();
        }
        catch (Exception)
        {
        }

        await database.CreateContainerAsync(new()
        {
            Id = DatabaseConstants.PLAYER_CONTAINER_NAME,
            PartitionKeyPath = "/lobbyId"
        });
    }
}
