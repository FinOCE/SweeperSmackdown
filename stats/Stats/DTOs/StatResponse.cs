namespace SweeperSmackdown.Stats.DTOs;

using Newtonsoft.Json;
using SweeperSmackdown.Stats.Models;
using System;

public class StatResponse(
    string id,
    int playtime,
    int[] tilesCleared,
    int minesFlagged,
    int minesHit,
    int blindGuesses,
    int boardsPlayed,
    int boardsCompleted,
    int boardsFailed,
    int gamesPlayed,
    int gamesWon,
    bool unlockedClassicMinesweeper,
    bool sharedMoment,
    int? fastestStandardSolve = null,
    int[]? largestBoardCompleted = null,
    int? largestZeroSpread = null,
    int? largestAdjacentBombCountFound = null,
    int? longestClearStreakWithoutMine = null,
    int? longestClearStreakWithoutMineOrFlag = null,
    int? fewestFlagsUsedWithoutHittingMine = null,
    DateTime? firstPlayed = null,
    DateTime? lastPlayed = null,
    int? largestLobbyPlayed = null)
{
    [JsonProperty("id")]
    public string Id { get; set; } = id;

    [JsonProperty("playtime")]
    public int Playtime { get; set; } = playtime;

    [JsonProperty("tilesCleared")]
    public int[] TilesCleared { get; set; } = tilesCleared;

    [JsonProperty("minesFlagged")]
    public int MinesFlagged { get; set; } = minesFlagged;

    [JsonProperty("minesHit")]
    public int MinesHit { get; set; } = minesHit;

    [JsonProperty("blindGuesses")]
    public int BlindGuesses { get; set; } = blindGuesses;

    [JsonProperty("fastestStandardSolve", NullValueHandling = NullValueHandling.Ignore)]
    public int? FastestStandardSolve { get; set; } = fastestStandardSolve;

    [JsonProperty("largestBoardCompleted", NullValueHandling = NullValueHandling.Ignore)]
    public int[]? LargestBoardCompleted { get; set; } = largestBoardCompleted;

    [JsonProperty("largestZeroSpread", NullValueHandling = NullValueHandling.Ignore)]
    public int? LargestZeroSpread { get; set; } = largestZeroSpread;

    [JsonProperty("largestAdjacentBombCountFound", NullValueHandling = NullValueHandling.Ignore)]
    public int? LargestAdjacentBombCountFound { get; set; } = largestAdjacentBombCountFound;

    [JsonProperty("longestClearStreakWithoutMine", NullValueHandling = NullValueHandling.Ignore)]
    public int? LongestClearStreakWithoutMine { get; set; } = longestClearStreakWithoutMine;

    [JsonProperty("longestClearStreakWithoutMineOrFlag", NullValueHandling = NullValueHandling.Ignore)]
    public int? LongestClearStreakWithoutMineOrFlag { get; set; } = longestClearStreakWithoutMineOrFlag;

    [JsonProperty("fewestFlagsUsedWithoutHittingMine", NullValueHandling = NullValueHandling.Ignore)]
    public int? FewestFlagsUsedWithoutHittingMine { get; set; } = fewestFlagsUsedWithoutHittingMine;

    [JsonProperty("boardsPlayed")]
    public int BoardsPlayed { get; set; } = boardsPlayed;

    [JsonProperty("boardsCompleted")]
    public int BoardsCompleted { get; set; } = boardsCompleted;

    [JsonProperty("boardsFailed")]
    public int BoardsFailed { get; set; } = boardsFailed;

    [JsonProperty("gamesPlayed")]
    public int GamesPlayed { get; set; } = gamesPlayed;

    [JsonProperty("gamesWon")]
    public int GamesWon { get; set; } = gamesWon;

    [JsonProperty("firstPlayed", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? FirstPlayed { get; set; } = firstPlayed;

    [JsonProperty("lastPlayed", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? LastPlayed { get; set; } = lastPlayed;

    [JsonProperty("unlockedClassicMinesweeper")]
    public bool UnlockedClassicMinesweeper { get; set; } = unlockedClassicMinesweeper;

    [JsonProperty("largestLobbyPlayed", NullValueHandling = NullValueHandling.Ignore)]
    public int? LargestLobbyPlayed { get; set; } = largestLobbyPlayed;

    [JsonProperty("sharedMoment")]
    public bool SharedMoment { get; set; } = sharedMoment;

    public static StatResponse FromModel(PlayerInfo playerInfo) => new(
        playerInfo.Id,
        playerInfo.Playtime,
        playerInfo.TilesCleared,
        playerInfo.MinesFlagged,
        playerInfo.MinesHit,
        playerInfo.BlindGuesses,
        playerInfo.BoardsPlayed,
        playerInfo.BoardsCompleted,
        playerInfo.BoardsFailed,
        playerInfo.GamesPlayed,
        playerInfo.GamesWon,
        playerInfo.UnlockedClassicMinesweeper,
        playerInfo.SharedMoment,
        playerInfo.FastestStandardSolve,
        playerInfo.LargestBoardCompleted,
        playerInfo.LargestZeroSpread,
        playerInfo.LargestAdjacentBombCountFound,
        playerInfo.LongestClearStreakWithoutMine,
        playerInfo.LongestClearStreakWithoutMineOrFlag,
        playerInfo.FewestFlagsUsedWithoutHittingMine,
        playerInfo.FirstPlayed,
        playerInfo.LastPlayed,
        playerInfo.LargestLobbyPlayed);
}
