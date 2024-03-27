using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.Structures;

public class GameSettings
{
    [JsonProperty("mode")]
    [JsonPropertyName("mode")]
    public int Mode { get; private set; } = 0;

    [JsonProperty("height")]
    [JsonPropertyName("height")]
    public int Height { get; private set; } = 16;

    [JsonProperty("width")]
    [JsonPropertyName("width")]
    public int Width { get; private set; } = 16;

    [JsonProperty("mines")]
    [JsonPropertyName("mines")]
    public int Mines { get; private set; } = 40;

    [JsonProperty("lives")]
    [JsonPropertyName("lives")]
    public int Lives { get; private set; } = 0;

    [JsonProperty("timeLimit")]
    [JsonPropertyName("timeLimit")]
    public int TimeLimit { get; private set; } = 0;

    [JsonProperty("boardCount")]
    [JsonPropertyName("boardCount")]
    public int BoardCount { get; private set; } = 0;

    [JsonProperty("seed")]
    [JsonPropertyName("seed")]
    public int Seed { get; private set; } = 0;

    public GameSettings Update(
        int? mode,
        int? height,
        int? width,
        int? mines,
        int? lives,
        int? timeLimit,
        int? boardCount,
        int? seed)
    {
        return new GameSettings()
        {
            Mode = mode ?? Mode,
            Height = height ?? Height,
            Width = width ?? Width,
            Mines = mines ?? Mines,
            Lives = lives ?? Lives,
            TimeLimit = timeLimit ?? TimeLimit,
            BoardCount = boardCount ?? BoardCount,
            Seed = seed ?? Seed
        };
    }
}
