namespace SweeperSmackdown.Stats.Models;

using Newtonsoft.Json;
using System;

public class PlayerInfo(
    string id,
    int? playtime = null,
    int[]? tilesCleared = null,
    int? minesFlagged = null,
    int? minesHit = null,
    int? blindGuesses = null,
    int? fastestStandardSolve = null,
    int[]? largestBoardCompleted = null,
    int? largestZeroSpread = null,
    int? largestAdjacentBombCountFound = null,
    int? longestClearStreakWithoutMine = null,
    int? longestClearStreakWithoutMineOrFlag = null,
    int? fewestFlagsUsedWithoutHittingMine = null,
    int? boardsPlayed = null,
    int? boardsCompleted = null,
    int? boardsFailed = null,
    int? gamesPlayed = null,
    int? gamesWon = null,
    DateTime? firstPlayed = null,
    DateTime? lastPlayed = null,
    bool? unlockedClassicMinesweeper = null,
    int? largestLobbyPlayed = null,
    bool? sharedMoment = null)
{
    [JsonProperty("id")]
    public string Id { get; set; } = id;

    [JsonProperty("playtime")]
    public int Playtime { get; set; } = playtime ?? 0; // Duration in seconds solving boards

    [JsonProperty("tilesCleared")]
    public int[] TilesCleared { get; set; } = tilesCleared ?? [0, 0, 0, 0, 0, 0, 0, 0, 0]; // Index = adjacent mine count

    [JsonProperty("minesFlagged")]
    public int MinesFlagged { get; set; } = minesFlagged ?? 0;

    [JsonProperty("minesHit")]
    public int MinesHit { get; set; } = minesHit ?? 0;

    [JsonProperty("blindGuesses")]
    public int BlindGuesses { get; set; } = blindGuesses ?? 0;

    [JsonProperty("fastestStandardSolve", NullValueHandling = NullValueHandling.Ignore)]
    public int? FastestStandardSolve { get; set; } = fastestStandardSolve;

    [JsonProperty("largestBoardCompleted", NullValueHandling = NullValueHandling.Ignore)]
    public int[]? LargestBoardCompleted { get; set; } = largestBoardCompleted; // [width, height], calculated based on volume

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
    public int BoardsPlayed { get; set; } = boardsPlayed ?? 0;

    [JsonProperty("boardsCompleted")]
    public int BoardsCompleted { get; set; } = boardsCompleted ?? 0;

    [JsonProperty("boardsFailed")]
    public int BoardsFailed { get; set; } = boardsFailed ?? 0;

    [JsonProperty("gamesPlayed")]
    public int GamesPlayed { get; set; } = gamesPlayed ?? 0;

    [JsonProperty("gamesWon")]
    public int GamesWon { get; set; } = gamesWon ?? 0;

    [JsonProperty("firstPlayed", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? FirstPlayed { get; set; } = firstPlayed;

    [JsonProperty("lastPlayed", NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? LastPlayed { get; set; } = lastPlayed;

    [JsonProperty("unlockedClassicMinesweeper")]
    public bool UnlockedClassicMinesweeper { get; set; } = unlockedClassicMinesweeper ?? false;

    [JsonProperty("largestLobbyPlayed", NullValueHandling = NullValueHandling.Ignore)]
    public int? LargestLobbyPlayed { get; set; } = largestLobbyPlayed;

    [JsonProperty("sharedMoment")]
    public bool SharedMoment { get; set; } = sharedMoment ?? false;
}
