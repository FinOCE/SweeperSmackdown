using SweeperSmackdown.Stats.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SweeperSmackdown.Stats.Assets;

public static class Constants
{
    public static IEnumerable<Achievement> Achievements { get; } =
    [
        new(
            "rookie",
            "Rookie",
            "Clear your first board",
            false,
            EAchievementTag.Win,
            EAchivementDifficulty.Easy,
            playerInfo => Math.Min(playerInfo.BoardsCompleted, 1)),
        new(
            "student",
            "Student",
            "Clear 10 boards",
            false,
            EAchievementTag.Win,
            EAchivementDifficulty.Medium,
            playerInfo => Math.Min(playerInfo.BoardsCompleted / 10m, 1)),
        new(
            "master",
            "Master",
            "Clear 100 boards",
            false,
            EAchievementTag.Win,
            EAchivementDifficulty.Hard,
            playerInfo => Math.Min(playerInfo.BoardsCompleted / 100m, 1)),
        new(
            "proficiency-1",
            "Sweeper Proficiency I",
            "Player Sweeper Smackdown for 1 hour",
            false,
            EAchievementTag.Progress,
            EAchivementDifficulty.Easy,
            playerInfo => playerInfo.Playtime / 60 * 60 * 1m),
        new(
            "proficiency-2",
            "Sweeper Proficiency II",
            "Player Sweeper Smackdown for 3 hours",
            false,
            EAchievementTag.Progress,
            EAchivementDifficulty.Medium,
            playerInfo => playerInfo.Playtime / 60 * 60 * 3m),
        new(
            "proficiency-3",
            "Sweeper Proficiency III",
            "Player Sweeper Smackdown for 10 hours",
            false,
            EAchievementTag.Progress,
            EAchivementDifficulty.Hard,
            playerInfo => playerInfo.Playtime / 60 * 60 * 10m),
        new(
            "proficiency-4",
            "Sweeper Proficiency IV",
            "Player Sweeper Smackdown for 100 hours",
            false,
            EAchievementTag.Progress,
            EAchivementDifficulty.Extreme,
            playerInfo => playerInfo.Playtime / 60 * 60 * 100m),
        new(
            "clearer-1",
            "Tile Clearer I",
            "Clear 1,000 tiles",
            false,
            EAchievementTag.Progress,
            EAchivementDifficulty.Easy,
            playerInfo => Math.Min(playerInfo.TilesCleared.Sum() / 1_000m, 1)),
        new(
            "clearer-2",
            "Tile Clearer II",
            "Clear 10,000 tiles",
            false,
            EAchievementTag.Progress,
            EAchivementDifficulty.Easy,
            playerInfo => Math.Min(playerInfo.TilesCleared.Sum() / 10_000m, 1)),
        new(
            "clearer-3",
            "Tile Clearer III",
            "Clear 100,000 tiles",
            false,
            EAchievementTag.Progress,
            EAchivementDifficulty.Easy,
            playerInfo => Math.Min(playerInfo.TilesCleared.Sum() / 100_000m, 1)),
        new(
            "lucky",
            "Lucky",
            "Get a zero-spread of 250 or more tiles",
            false,
            EAchievementTag.Event,
            EAchivementDifficulty.Medium,
            playerInfo => Math.Min((playerInfo.LargestZeroSpread ?? 0) / 250m, 1)),
        new(
            "going-in-blind",
            "Going in Blind",
            "Make 100 blind guesses",
            false,
            EAchievementTag.Mechanic,
            EAchivementDifficulty.Medium,
            playerInfo => Math.Min(playerInfo.BlindGuesses / 100m, 1)),
        new(
            "surround-7",
            "One Right Move",
            "Find a tile surrounded by 7 mines",
            false,
            EAchievementTag.Event,
            EAchivementDifficulty.Hard,
            playerInfo => Math.Min((playerInfo.LargestAdjacentBombCountFound ?? 0) / 7m, 1)),
        new(
            "surround-8",
            "Magic Eight-Ball",
            "Find a tile surrounded by 8 mines",
            false,
            EAchievementTag.Event,
            EAchivementDifficulty.Extreme,
            playerInfo => Math.Min((playerInfo.LargestAdjacentBombCountFound ?? 0) / 8m, 1)),
        new(
            "endurance-athlete",
            "Endurance Athlete",
            "Clear a 32x32 or larger board",
            false,
            EAchievementTag.Challenge,
            EAchivementDifficulty.Hard,
            playerInfo => (playerInfo.LargestBoardCompleted ?? [0, 0]).All(size => size >= 32) ? 1 : 0),
        new(
            "no-hints",
            "No Hints",
            "Clear a board without using any flags or hitting mines",
            false,
            EAchievementTag.Challenge,
            EAchivementDifficulty.Medium,
            playerInfo => (playerInfo.FewestFlagsUsedWithoutHittingMine ?? -1) == 0 ? 1 : 0),
        new(
            "flagger",
            "X Marks the Spot",
            "Place 1000 flags on mines",
            false,
            EAchievementTag.Style,
            EAchivementDifficulty.Hard,
            playerInfo => Math.Min(playerInfo.MinesFlagged / 1000m, 1)),
        new(
            "classic-minesweeper",
            "1992",
            "Unlock the classic Minesweeper style",
            false,
            EAchievementTag.Event,
            EAchivementDifficulty.Easy,
            playerInfo => playerInfo.UnlockedClassicMinesweeper ? 1 : 0),
        new(
            "lobby-size-2",
            "Head-to-head",
            "Play in a lobby with at least one other player",
            false,
            EAchievementTag.Style,
            EAchivementDifficulty.Easy,
            playerInfo => (playerInfo.LargestLobbyPlayed ?? 0) >= 2 ? 1 : 0),
        new(
            "lobby-size-3",
            "Third Wheeling",
            "Play in a lobby with at least two other players",
            false,
            EAchievementTag.Style,
            EAchivementDifficulty.Medium,
            playerInfo => (playerInfo.LargestLobbyPlayed ?? 0) >= 3 ? 1 : 0),
        new(
            "lobby-size-5",
            "Party People",
            "Play in a lobby with at least four other players",
            false,
            EAchievementTag.Style,
            EAchivementDifficulty.Hard,
            playerInfo => (playerInfo.LargestLobbyPlayed ?? 0) >= 5 ? 1 : 0),
        new(
            "sharing",
            "Sharing is Caring",
            "Share a result with your friends",
            false,
            EAchievementTag.Event,
            EAchivementDifficulty.Easy,
            playerInfo => playerInfo.SharedMoment ? 1 : 0),
        new(
            "speed",
            "I am Speed",
            "Complete a 16x16 board in under a minute",
            false,
            EAchievementTag.Challenge,
            EAchivementDifficulty.Hard,
            playerInfo => (playerInfo.FastestStandardSolve ?? 61) < 60 ? 1 : 0)
    ];
}

public enum EAchievementTag
{
    Win,
    Event,
    Challenge,
    Progress,
    Style,
    Mechanic
}

public enum EAchivementDifficulty
{
    Easy,
    Medium,
    Hard,
    Extreme
}
