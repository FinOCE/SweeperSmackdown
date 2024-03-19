using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace SweeperSmackdown.Utils;

public static class Id
{
    /// <summary>
    /// Get the ID for a given entity based on the game instance ID.
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="instanceId">The game instance ID</param>
    /// <returns>The EntityId for the given entity</returns>
    public static EntityId For<T>(string instanceId) =>
        new(typeof(T).Name, instanceId);

    /// <summary>
    /// Get the ID for a given entity based on the game instance ID and user ID.
    /// </summary>
    /// <typeparam name="T">The type of entity</typeparam>
    /// <param name="instanceId">The game instance ID</param>
    /// <param name="userId">The user ID</param>
    /// <returns>The EntityId for the given entity</returns>
    public static EntityId For<T>(string instanceId, string userId) =>
        new(typeof(T).Name, $"{instanceId}:{userId}");

    /// <summary>
    /// Get the ID for an instance of a given type.
    /// </summary>
    /// <param name="nameof">The name of the type</param>
    /// <param name="instanceId">The game instance ID</param>
    /// <returns>The ID for an instance of the type for the given instance ID</returns>
    public static string ForInstance(string nameof, string instanceId) =>
        $"{nameof}:{instanceId}";
}
