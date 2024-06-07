using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Models;
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

    public static async Task<IEnumerable<Lobby>> ChangeHostAsync(this CosmosClient cosmosClient, string userId)
    {
        var lobbies = await cosmosClient
            .GetPlayerContainer()
            .GetItemLinqQueryable<Lobby>()
                .Where(l => l.HostId == userId)
            .ToFeedIterator()
            .ReadAllAsync();

        var updatedLobbies = await Task.WhenAll(
            lobbies.Select(lobby => Task.Run(async () => {
                var players = await cosmosClient.GetAllPlayersInLobbyAsync(lobby.Id);

                if (players.Any())
                {
                    var hostId = players.First().Id;

                    await cosmosClient.GetLobbyContainer().PatchItemAsync<Lobby>(lobby.Id, new(lobby.Id), new[]
                    {
                        PatchOperation.Set("/hostId", hostId)
                    });
                    lobby.HostId = hostId;

                    return lobby;
                }

                return null;
            })));

        return updatedLobbies.Where(l => l is not null) as IEnumerable<Lobby>;
    }
}
