using Newtonsoft.Json;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Utils;
using System;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.Structures;

public class GameSettings
{
    [JsonProperty("mode")]
    [JsonPropertyName("mode")]
    public int Mode { get; private set; } = Constants.DEFAULT_GAME_MODE;

    [JsonProperty("height")]
    [JsonPropertyName("height")]
    public int Height { get; private set; } = Constants.DEFAULT_GAME_HEIGHT;

    [JsonProperty("width")]
    [JsonPropertyName("width")]
    public int Width { get; private set; } = Constants.DEFAULT_GAME_WIDTH;

    [JsonProperty("mines")]
    [JsonPropertyName("mines")]
    public int Mines { get; private set; } = MineUtils.CalculateMineCount(
        Constants.DEFAULT_GAME_DIFFICULTY,
        Constants.DEFAULT_GAME_HEIGHT * Constants.DEFAULT_GAME_WIDTH);

    [JsonProperty("difficulty")]
    [JsonPropertyName("difficulty")]
    public EDifficulty? Difficulty { get; private set; } = Constants.DEFAULT_GAME_DIFFICULTY;

    [JsonProperty("lives")]
    [JsonPropertyName("lives")]
    public int Lives { get; private set; } = Constants.DEFAULT_GAME_LIVES;

    [JsonProperty("timeLimit")]
    [JsonPropertyName("timeLimit")]
    public int TimeLimit { get; private set; } = Constants.DEFAULT_GAME_TIME_LIMIT;

    [JsonProperty("boardCount")]
    [JsonPropertyName("boardCount")]
    public int BoardCount { get; private set; } = Constants.DEFAULT_GAME_BOARD_COUNT;

    [JsonProperty("seed")]
    [JsonPropertyName("seed")]
    public int Seed { get; private set; } = Constants.DEFAULT_GAME_SEED;

    public GameSettings() { }

    public GameSettings(int seed)
    {
        Seed = seed;
    }

    public GameSettings Update(
        int? mode,
        int? height,
        int? width,
        int? mines,
        EDifficulty? difficulty,
        int? lives,
        int? timeLimit,
        int? boardCount,
        int? seed)
    {
        var usesDifficulty = (difficulty ?? Difficulty) is not null && mines == null;

        var newDifficulty = usesDifficulty
            ? (difficulty ?? Difficulty)
            : null;

        var newMines = newDifficulty.HasValue
            ? MineUtils.CalculateMineCount(newDifficulty.Value, (height ?? Height) * (width ?? Width))
            : mines ?? Mines;

        if (newMines > (height ?? Height) * (width ?? Width))
            throw new ArgumentException("Too many mines");
        
        return new GameSettings()
        {
            Mode = mode ?? Mode,
            Height = height ?? Height,
            Width = width ?? Width,
            Mines = newMines,
            Difficulty = newDifficulty,
            Lives = lives ?? Lives,
            TimeLimit = timeLimit ?? TimeLimit,
            BoardCount = boardCount ?? BoardCount,
            Seed = seed ?? Seed
        };
    }
}
