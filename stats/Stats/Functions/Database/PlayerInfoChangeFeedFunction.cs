using Azure.Messaging.EventGrid;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using SweeperSmackdown.Stats.Assets;
using SweeperSmackdown.Stats.DTOs.Events;
using SweeperSmackdown.Stats.Models;
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
        var progresses = infos.Select(info =>
            new AchievementProgressCollection(
                info.Id,
                Constants.Achievements.Select(achievement =>
                    new AchievementProgress(achievement.Id, achievement.GetProgress(info)))));

        // Calculate new progression and completion of achievements
        var updated = progresses
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
                progresses
                    .Where(n => !oldProgresses.Any(o => o.Id == n.Id))
                    .Select(n => KeyValuePair.Create(
                        n.Id,
                        n.Achievements.Where(a => a.Progress != 0))))
            .Where(n => n.Value.Any())
            .ToDictionary();

        // Notify of updates to achievements and update db
        var events = updated.Select(kvp => new EventGridEvent(
            kvp.Key,
            "SweeperSmackdown.Achievements.Updated",
            "1.0.0",
            new AchievementsUpdatedEventResponse(
                kvp.Key,
                kvp.Value.Where(a => a.Progress != 1),
                kvp.Value.Where(a => a.Progress == 1))));

        return new(progresses, events);
    }
}
