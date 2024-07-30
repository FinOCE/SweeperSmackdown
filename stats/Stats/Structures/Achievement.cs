using SweeperSmackdown.Stats.Assets;
using SweeperSmackdown.Stats.Models;
using System;

namespace SweeperSmackdown.Stats.Structures;

public class Achievement(
    string id,
    string name,
    string description,
    EAchievementTag tag,
    EAchivementDifficulty difficulty,
    Func<PlayerInfo, decimal> getProgress)
{
    public string Id { get; } = id;

    public string Name { get; } = name;

    public string Description { get; } = description;

    public EAchievementTag Tag { get; } = tag;

    public EAchivementDifficulty Difficulty { get; } = difficulty;

    public Func<PlayerInfo, decimal> GetProgress { get; } = getProgress;
}
