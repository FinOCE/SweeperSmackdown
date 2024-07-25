using Newtonsoft.Json;

namespace SweeperSmackdown.Stats.Models;

public class AchievementProgress(string id, decimal progress)
{
    [JsonProperty("id")]
    public string Id { get; set; } = id;

    [JsonProperty("progress")]
    public decimal Progress { get; set; } = progress;
}
