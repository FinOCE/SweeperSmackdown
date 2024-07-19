using Newtonsoft.Json;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Utils;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.Structures;

public class GameSettings
{
    [JsonProperty("mode")]
    [JsonPropertyName("mode")]
    public int Mode { get; set; } = Constants.DEFAULT_GAME_MODE;

    [JsonProperty("height")]
    [JsonPropertyName("height")]
    public int Height { get; set; } = Constants.DEFAULT_GAME_HEIGHT;

    [JsonProperty("width")]
    [JsonPropertyName("width")]
    public int Width { get; set; } = Constants.DEFAULT_GAME_WIDTH;

    [JsonProperty("mines")]
    [JsonPropertyName("mines")]
    public int Mines { get; set; } = MineUtils.CalculateMineCount(
        Constants.DEFAULT_GAME_DIFFICULTY,
        Constants.DEFAULT_GAME_HEIGHT * Constants.DEFAULT_GAME_WIDTH);

    [JsonProperty("difficulty")]
    [JsonPropertyName("difficulty")]
    public EDifficulty? Difficulty { get; set; } = Constants.DEFAULT_GAME_DIFFICULTY;

    [JsonProperty("lives")]
    [JsonPropertyName("lives")]
    public int Lives { get; set; } = Constants.DEFAULT_GAME_LIVES;

    [JsonProperty("timeLimit")]
    [JsonPropertyName("timeLimit")]
    public int TimeLimit { get; set; } = Constants.DEFAULT_GAME_TIME_LIMIT;

    [JsonProperty("boardCount")]
    [JsonPropertyName("boardCount")]
    public int BoardCount { get; set; } = Constants.DEFAULT_GAME_BOARD_COUNT;

    [JsonProperty("seed")]
    [JsonPropertyName("seed")]
    public int Seed { get; set; } = Constants.DEFAULT_GAME_SEED;

    public GameSettings() { }

    public GameSettings(int seed)
    {
        Seed = seed;
    }
}
