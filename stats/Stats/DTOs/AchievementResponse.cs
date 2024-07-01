using Newtonsoft.Json;
using SweeperSmackdown.Stats.Assets;
using SweeperSmackdown.Stats.Structures;

namespace SweeperSmackdown.Stats.DTOs;

public class AchievementResponse(
    string id,
    string name,
    string description,
    EAchievementTag tag,
    EAchivementDifficulty difficulty)
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

    public static AchievementResponse FromModel(Achievement achievement) =>
        new(
            achievement.Id,
            achievement.Name,
            achievement.Description,
            achievement.Tag,
            achievement.Difficulty);
}
