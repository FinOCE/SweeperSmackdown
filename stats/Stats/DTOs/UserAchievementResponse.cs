using Newtonsoft.Json;
using SweeperSmackdown.Stats.Assets;
using SweeperSmackdown.Stats.Models;
using SweeperSmackdown.Stats.Structures;

namespace SweeperSmackdown.Stats.DTOs;

public class UserAchievementResponse(
    string id,
    string name,
    string description,
    EAchievementTag tag,
    EAchivementDifficulty difficulty,
    decimal progress)
{
    [JsonProperty("id")]
    public string Id { get; set; } = id;

    [JsonProperty("name")]
    public string Name { get; set; } = name;

    [JsonProperty("description")]
    public string Description { get; set; } = description;

    [JsonProperty("tag")]
    public EAchievementTag Tag { get; set; } = tag;

    [JsonProperty("difficulty")]
    public EAchivementDifficulty Difficulty { get; set; } = difficulty;

    [JsonProperty("progress")]
    public decimal Progress { get; set; } = progress;

    public static UserAchievementResponse FromModel(Achievement achievement, AchievementProgress? progress) =>
        new(
            achievement.Id,
            achievement.Name,
            achievement.Description,
            achievement.Tag,
            achievement.Difficulty,
            progress is not null ? progress.Progress : 0);
}
