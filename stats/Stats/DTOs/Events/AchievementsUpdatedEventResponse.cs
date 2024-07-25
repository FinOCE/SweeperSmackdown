using Newtonsoft.Json;
using SweeperSmackdown.Stats.Models;
using System.Collections.Generic;

namespace SweeperSmackdown.Stats.DTOs.Events;

public class AchievementsUpdatedEventResponse(
    string id,
    IEnumerable<AchievementProgress> progressed,
    IEnumerable<AchievementProgress> completed)
{
    [JsonProperty("id")]
    public string Id { get; set; } = id;

    [JsonProperty("progressed")]
    public IEnumerable<AchievementProgress> Progressed { get; set; } = progressed;

    [JsonProperty("completed")]
    public IEnumerable<AchievementProgress> Completed { get; set; } = completed;
}
