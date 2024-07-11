using Newtonsoft.Json;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;

namespace SweeperSmackdown.DTOs;

public class GameSettingsResponse
{
    [JsonProperty("mode")]
    public int Mode { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("mines")]
    public int Mines { get; set; }

    [JsonProperty("difficulty")]
    public EDifficulty? Difficulty { get; set; }

    [JsonProperty("lives")]
    public int? Lives { get; set; }

    [JsonProperty("timeLimit")]
    public int? TimeLimit { get; set; }

    [JsonProperty("boardCount")]
    public int? BoardCount { get; set; }

    [JsonProperty("shareBoards")]
    public bool ShareBoards { get; set; }

    public GameSettingsResponse(
        int mode,
        int height,
        int width,
        int mines,
        EDifficulty? difficulty,
        int? lives,
        int? timeLimit,
        int? boardCount,
        bool shareBoards)
    {
        Mode = mode;
        Height = height;
        Width = width;
        Mines = mines;
        Difficulty = difficulty;
        Lives = lives;
        TimeLimit = timeLimit;
        BoardCount = boardCount;
        ShareBoards = shareBoards;
    }

    public static GameSettingsResponse FromModel(GameSettings model) =>
        new(
            model.Mode,
            model.Height,
            model.Width,
            model.Mines,
            model.Difficulty,
            model.Lives == 0 ? null : model.Lives,
            model.TimeLimit == 0 ? null : model.TimeLimit,
            model.BoardCount == 0 ? null : model.BoardCount,
            model.Seed != 0);
}
