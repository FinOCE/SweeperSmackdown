using Newtonsoft.Json;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;

namespace SweeperSmackdown.DTOs;

public class GameSettingsUpdateRequest
{
    [JsonProperty("mode")]
    public int? Mode { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }

    [JsonProperty("width")]
    public int? Width { get; set; }

    [JsonProperty("mines")]
    public int? Mines { get; set; }

    [JsonProperty("difficulty")]
    public EDifficulty? Difficulty { get; set; }

    [JsonProperty("lives")]
    public int? Lives { get; set; }

    [JsonProperty("timeLimit")]
    public int? TimeLimit { get; set; }

    [JsonProperty("boardCount")]
    public int? BoardCount { get; set; }

    [JsonProperty("shareBoards")]
    public bool? ShareBoards { get; set; }

    public GameSettings ApplyToModel(GameSettings model)
    {
        var usesDifficulty = (Difficulty ?? model.Difficulty) is not null && Mines == null;

        var newDifficulty = usesDifficulty
            ? (Difficulty ?? model.Difficulty)
            : null;

        var newMines = newDifficulty.HasValue
            ? MineUtils.CalculateMineCount(newDifficulty.Value, (Height ?? model.Height) * (Width ?? model.Width))
            : Mines ?? model.Mines;

        if (newMines > (Height ?? model.Height) * (Width ?? model.Width))
            throw new ArgumentException("Too many mines");

        var newSeed = ShareBoards != null
            ? (ShareBoards.Value ? Guid.NewGuid().GetHashCode() : 0)
            : model.Seed;

        model.Mode = Mode ?? model.Mode;
        model.Height = Height ?? model.Height;
        model.Width = Width ?? model.Width;
        model.Difficulty = newDifficulty;
        model.Mines = newMines;
        model.Lives = Lives ?? model.Lives;
        model.TimeLimit = TimeLimit ?? model.TimeLimit;
        model.BoardCount = BoardCount ?? model.BoardCount;
        model.Seed = newSeed;

        return model;
    }
}
