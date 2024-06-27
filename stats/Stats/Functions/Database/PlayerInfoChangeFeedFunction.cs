using Azure.Messaging.EventGrid;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using SweeperSmackdown.Stats.Assets;
using SweeperSmackdown.Stats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Stats.Functions.Database;

public class PlayerInfoChangeFeedFunctionOutputs(
    IEnumerable<AchievementProgressCollection> achievementProgresses,
    IEnumerable<EventGridEvent> events)
{
    [CosmosDBOutput(Constants.DATABASE_NAME, Constants.ACHIEVEMENTS_CONTAINER_NAME)]
    public IEnumerable<AchievementProgressCollection> AchievementProgresses { get; set; } = achievementProgresses;

    [EventGridOutput]
    public IEnumerable<EventGridEvent> Events { get; set; } = events;
}

public static class PlayerInfoChangeFeedFunction
{
    [Function(nameof(PlayerInfoChangeFeedFunction))]
    
    public static async Task<PlayerInfoChangeFeedFunctionOutputs> Run(
        [CosmosDBTrigger(Constants.DATABASE_NAME, Constants.STATS_CONTAINER_NAME, CreateLeaseContainerIfNotExists = true)] IEnumerable<PlayerInfo> infos,
        [CosmosDBInput(Constants.DATABASE_NAME, Constants.ACHIEVEMENTS_CONTAINER_NAME)] Container container)
    {
        // Fetch existing achievement progresses
        var keys = infos
            .Select(info => (info.Id, new PartitionKey(info.Id)))
            .ToList();

        IEnumerable<AchievementProgressCollection> oldProgresses = await container
            .ReadManyItemsAsync<AchievementProgressCollection>(keys);

        // Calculate new achievement progresses
        var newProgresses = infos.Select(info =>
            new AchievementProgressCollection(
                info.Id,
                Constants.Achievements.Select(achievement =>
                    new AchievementProgress(achievement.Id, achievement.GetProgress(info)))));

        // Calculate new progression and completion of achievements
        var newlyUpdated = newProgresses
            .Where(n =>
                oldProgresses
                    .Any(o => o.Id == n.Id))
                    .Select(n => KeyValuePair.Create(
                        n.Id,
                        n.Achievements
                            .Where(na =>
                                na.Progress != 0 &&
                                !oldProgresses
                                    .First(o => o.Id == n.Id).Achievements
                                    .Any(oa => oa.Id == na.Id && oa.Progress == na.Progress))))
            .Concat(
                newProgresses
                    .Where(n => !oldProgresses.Any(o => o.Id == n.Id))
                    .Select(n => KeyValuePair.Create(
                        n.Id,
                        n.Achievements.Where(a => a.Progress != 0))))
            .Where(n => n.Value.Any())
            .ToDictionary();

        var newlyProgressed = newlyUpdated
            .Select(kvp => KeyValuePair.Create(
                kvp.Key,
                kvp.Value.Where(a => a.Progress != 1)))
            .Where(kvp => kvp.Value.Any())
            .ToDictionary();

        var newlyCompleted = newlyUpdated
            .Select(kvp => KeyValuePair.Create(
                kvp.Key,
                kvp.Value.Where(a => a.Progress == 1)))
            .Where(kvp => kvp.Value.Any())
            .ToDictionary();

        // Notify of progressed and completed achievements and update achievement db
        var progressedEvents = newlyProgressed.SelectMany(u =>
            u.Value.Select(v =>
                new EventGridEvent(
                    u.Key,
                    "SweeperSmackdown.Achievements.Progressed",
                    "1.0.0",
                    v)));

        var completedEvents = newlyCompleted.SelectMany(u =>
            u.Value.Select(v =>
                new EventGridEvent(
                    u.Key,
                    "SweeperSmackdown.Achievements.Completed",
                    "1.0.0",
                    v)));

        return new(
            newProgresses,
            Array.Empty<EventGridEvent>()
                .Concat(progressedEvents)
                .Concat(completedEvents));
    }
}
