using Newtonsoft.Json;
using System.Collections.Generic;

namespace SweeperSmackdown.Stats.Models;

public class AchievementProgressCollection(string id, IEnumerable<AchievementProgress> achivements)
{
    [JsonProperty("id")]
    public string Id { get; set; } = id;

    [JsonProperty("achievements")]
    public IEnumerable<AchievementProgress> Achievements { get; set; } = achivements;
}
