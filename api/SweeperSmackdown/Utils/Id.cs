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
    /// Get the ID for an orchestrator of a given type.
    /// </summary>
    /// <param name="nameof">The name of the type</param>
    /// <param name="instanceId">The game instance ID</param>
    /// <returns>The ID for an instance of the type for the given instance ID</returns>
    public static string ForInstance(string nameof, string instanceId) =>
        $"{nameof}:{instanceId}";

    /// <summary>
    /// Get the ID for an orchestrator of a given type and user ID.
    /// </summary>
    /// <param name="nameof">The name of the type</param>
    /// <param name="instanceId">The game instance ID</param>
    /// <param name="userId">The user's ID</param>
    /// <returns>The ID for an instance of the type for the given instance ID and user ID</returns>
    public static string ForInstance(string nameof, string instanceId, string userId) =>
        $"{nameof}:{instanceId}:{userId}";

    /// <summary>
    /// Get the game lobby ID from an orchestrator instance ID.
    /// </summary>
    /// <param name="instanceId">The orchestrator's instance ID</param>
    /// <returns>The game lobby ID</returns>
    public static string FromInstance(string instanceId) =>
        instanceId.Split(":")[1];

    /// <summary>
    /// Get the user ID from an orchestrator scoped to a specific user.
    /// </summary>
    /// <param name="instanceId">The orchestrator's instance ID</param>
    /// <returns>The user's ID</returns>
    public static string UserFromInstance(string instanceId) =>
        instanceId.Split(":")[2];

    /// <summary>
    /// Get the ID for a user by combining a base user ID with the connection ID.
    /// </summary>
    /// <param name="userId">The user's base ID</param>
    /// <param name="connectionId">The user's connection ID</param>
    /// <returns>The composite user ID</returns>
    public static string ForUser(string userId, string connectionId) =>
        $"{userId}_{connectionId}";

    /// <summary>
    /// Get the base user ID and connection ID from a composite ID.
    /// </summary>
    /// <param name="id">The composite user ID</param>
    /// <returns>The user ID and connection ID</returns>
    public static (string UserId, string? ConnectionId) FromUser(string id)
    {
        var parts = id.Split("_");
        return (parts[0], parts.Length > 1 ? parts[1] : null);

        // TODO: Once fully implemented, make ConnectionId not nullable
    }
}
