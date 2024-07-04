using Newtonsoft.Json;
using SweeperSmackdown.Stats.Models;
using System;
using System.Linq;

namespace SweeperSmackdown.Stats.DTOs;

public class StatRequest(
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
    DateTime? lastPlayed = null,
    bool? unlockedClassicMinesweeper = null,
    int? lobbySize = null,
    bool? sharedMoment = null)
{
    [JsonProperty("id")]
    public string Id { get; set; } = id;

    [JsonProperty("playtime")]
    public int? Playtime { get; set; } = playtime;

    [JsonProperty("tilesCleared")]
    public int[]? TilesCleared { get; set; } = tilesCleared;

    [JsonProperty("minesFlagged")]
    public int? MinesFlagged { get; set; } = minesFlagged;

    [JsonProperty("minesHit")]
    public int? MinesHit { get; set; } = minesHit;

    [JsonProperty("blindGuesses")]
    public int? BlindGuesses { get; set; } = blindGuesses;

    [JsonProperty("fastestStandardSolve")]
    public int? FastestStandardSolve { get; set; } = fastestStandardSolve;

    [JsonProperty("largestBoardCompleted")]
    public int[]? LargestBoardCompleted { get; set; } = largestBoardCompleted;

    [JsonProperty("largestZeroSpread")]
    public int? LargestZeroSpread { get; set; } = largestZeroSpread;

    [JsonProperty("largestAdjacentBombCountFound")]
    public int? LargestAdjacentBombCountFound { get; set; } = largestAdjacentBombCountFound;

    [JsonProperty("longestClearStreakWithoutMine")]
    public int? LongestClearStreakWithoutMine { get; set; } = longestClearStreakWithoutMine;

    [JsonProperty("longestClearStreakWithoutMineOrFlag")]
    public int? LongestClearStreakWithoutMineOrFlag { get; set; } = longestClearStreakWithoutMineOrFlag;

    [JsonProperty("fewestFlagsUsedWithoutHittingMine")]
    public int? FewestFlagsUsedWithoutHittingMine { get; set; } = fewestFlagsUsedWithoutHittingMine;

    [JsonProperty("boardsPlayed")]
    public int? BoardsPlayed { get; set; } = boardsPlayed;

    [JsonProperty("boardsCompleted")]
    public int? BoardsCompleted { get; set; } = boardsCompleted;

    [JsonProperty("boardsFailed")]
    public int? BoardsFailed { get; set; } = boardsFailed;

    [JsonProperty("gamesPlayed")]
    public int? GamesPlayed { get; set; } = gamesPlayed;

    [JsonProperty("gamesWon")]
    public int? GamesWon { get; set; } = gamesWon;

    [JsonProperty("lastPlayed")]
    public DateTime? LastPlayed { get; set; } = lastPlayed;

    [JsonProperty("unlockedClassicMinesweeper")]
    public bool? UnlockedClassicMinesweeper { get; set; } = unlockedClassicMinesweeper;

    [JsonProperty("lobbySize")]
    public int? LobbySize { get; set; } = lobbySize;

    [JsonProperty("sharedMoment")]
    public bool? SharedMoment { get; set; } = sharedMoment;

    public PlayerInfo ApplyToModel(PlayerInfo? model)
    {
        model ??= new(Id);

        if (Playtime is not null)
            model.Playtime += Playtime.Value;

        if (TilesCleared is not null)
            model.TilesCleared = model.TilesCleared.Select((v, i) => v + TilesCleared[i]).ToArray();

        if (MinesFlagged is not null)
            model.MinesFlagged += MinesFlagged.Value;

        if (MinesHit is not null)
            model.MinesHit += MinesHit.Value;

        if (BlindGuesses is not null)
            model.BlindGuesses += BlindGuesses.Value;

        if (FastestStandardSolve is not null && FastestStandardSolve.Value < model.FastestStandardSolve)
            model.FastestStandardSolve = FastestStandardSolve;

        if (LargestBoardCompleted is not null)
        {
            var currentArea = model.LargestBoardCompleted is not null
                ? model.LargestBoardCompleted[0] * model.LargestBoardCompleted[1]
                : 0;

            var newArea = LargestBoardCompleted[0] * LargestBoardCompleted[1];

            if (newArea > currentArea)
                model.LargestBoardCompleted = LargestBoardCompleted;
        }

        if (LargestZeroSpread is not null && LargestZeroSpread > model.LargestZeroSpread)
            model.LargestZeroSpread = LargestZeroSpread;

        if (LargestAdjacentBombCountFound is not null && LargestAdjacentBombCountFound > model.LargestAdjacentBombCountFound)
            model.LargestAdjacentBombCountFound = LargestAdjacentBombCountFound;

        if (LongestClearStreakWithoutMine is not null && LongestClearStreakWithoutMine > model.LongestClearStreakWithoutMine)
            model.LongestClearStreakWithoutMine = LongestClearStreakWithoutMine;

        if (LongestClearStreakWithoutMineOrFlag is not null && LongestClearStreakWithoutMineOrFlag > model.LongestClearStreakWithoutMineOrFlag)
            model.LongestClearStreakWithoutMineOrFlag = LongestClearStreakWithoutMineOrFlag;

        if (BoardsPlayed is not null)
            model.BoardsPlayed += BoardsPlayed.Value;

        if (BoardsCompleted is not null)
            model.BoardsCompleted += BoardsCompleted.Value;

        if (BoardsFailed is not null)
            model.BoardsFailed += BoardsFailed.Value;

        if (GamesPlayed is not null)
            model.GamesPlayed += GamesPlayed.Value;

        if (GamesWon is not null)
            model.GamesWon += GamesWon.Value;

        if (LastPlayed is not null)
        {
            model.FirstPlayed ??= LastPlayed;

            if (model.LastPlayed < LastPlayed)
                model.LastPlayed = LastPlayed;
        }

        if (UnlockedClassicMinesweeper is not null && UnlockedClassicMinesweeper.Value)
            model.UnlockedClassicMinesweeper = UnlockedClassicMinesweeper.Value;

        if (LobbySize is not null && LobbySize.Value > model.LargestLobbyPlayed)
            model.LargestLobbyPlayed = LobbySize.Value;

        if (SharedMoment is not null && SharedMoment.Value)
            model.SharedMoment = SharedMoment.Value;

        return model;
    }
}
