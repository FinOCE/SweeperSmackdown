using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Extensions;

public static class IDurableEntityClientExtensions
{
    public const int DEFAULT_INTERVAL_MILLISECONDS = 100;

    public const int DEFAULT_TIMEOUT_MILLISECONDS = 10 * 1000;

    /// <summary>
    /// Poll the entity until an update operation has completed. The provided callback is used to determine if the
    /// update has been completed.
    /// </summary>
    /// <typeparam name="T">The type of the entity being updated</typeparam>
    /// <param name="entityClient">The entity client</param>
    /// <param name="entityId">The ID of the entity being updated</param>
    /// <param name="updateChanged">A callback to determine if the update has been applied</param>
    /// <param name="interval">How long to wait between polling (defaults to 100 milliseconds)</param>
    /// <param name="timeout">How long to wait before giving up on polling (defaults to 10 seconds)</param>
    /// <returns>The updated entity</returns>
    /// <exception cref="TimeoutException">Occurs when the timeout is reached before update is completed</exception>
    /// <exception cref="NullReferenceException">Occurs when the state is null after change is completed</exception>
    public static async Task<T> WaitForUpdateAsync<T>(
        this IDurableEntityClient entityClient,
        EntityId entityId,
        Func<T, bool> updateChanged,
        TimeSpan? interval = null,
        TimeSpan? timeout = null)
    {
        interval ??= TimeSpan.FromMilliseconds(DEFAULT_INTERVAL_MILLISECONDS);
        timeout ??= TimeSpan.FromMilliseconds(DEFAULT_TIMEOUT_MILLISECONDS);

        var hasChanged = false;
        T? state = default;

        var startTime = DateTime.UtcNow;

        while (!hasChanged)
        {
            var entityState = await entityClient.ReadEntityStateAsync<T>(entityId);
            state = entityState.EntityState;

            if (entityState.EntityExists)
                hasChanged = updateChanged(entityState.EntityState);

            if (startTime.Add(timeout.Value) < DateTime.UtcNow)
                throw new TimeoutException($"State of polled entity {entityId} did not update before timeout");

            await Task.Delay(interval.Value);
        }

        if (state is null)
            throw new NullReferenceException($"State of polled entity {entityId} was unexpectedly null");

        return state;
    }
}
