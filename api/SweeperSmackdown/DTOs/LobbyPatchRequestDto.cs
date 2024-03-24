using Newtonsoft.Json;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using System.Collections.Generic;
using System.Linq;

namespace SweeperSmackdown.DTOs;

public class LobbyPatchRequestDto
{

    [JsonProperty("hostId")]
    public string? HostId { get; }
    
    [JsonProperty("mode")]
    public int? Mode { get; }

    [JsonProperty("height")]
    public int? Height { get; }

    [JsonProperty("width")]
    public int? Width { get; }

    [JsonProperty("mines")]
    public int? Mines { get; }

    [JsonProperty("lives")]
    public int? Lives { get; }

    [JsonProperty("timeLimit")]
    public int? TimeLimit { get; }

    [JsonProperty("boardCount")]
    public int? BoardCount { get; }

    [JsonProperty("shareBoards")]
    public bool? ShareBoards { get; }

    [JsonIgnore]
    public bool IsValid
    {
        get { return Errors.Length == 0; }
    }

    [JsonIgnore]
    public string[] Errors
    {
        get
        {
            var errors = new List<string>();

            if (Mode != null && !GameStateFactory.VALID_MODES.Contains(Mode.Value))
                errors.Add($"The 'mode' is not a valid option ({string.Join(", ", GameStateFactory.VALID_MODES)})");

            if (Height != null && Height >= Constants.MAX_GAME_HEIGHT || Height <= Constants.MIN_GAME_HEIGHT)
                errors.Add($"The 'height' must be between {Constants.MIN_GAME_HEIGHT} and {Constants.MAX_GAME_HEIGHT}");

            if (Width != null && Width >= Constants.MAX_GAME_WIDTH || Width <= Constants.MIN_GAME_WIDTH)
                errors.Add($"The 'width' must be between {Constants.MIN_GAME_WIDTH} and {Constants.MAX_GAME_WIDTH}");

            if (Mines != null && Mines <= 0)
                errors.Add($"The 'mines' must be greater than 0");

            if (Lives != null && Lives < 0)
                errors.Add($"The 'lives' must be greater than or equal to 0 (0 means unlimited)");

            if (TimeLimit != null && TimeLimit < 0)
                errors.Add("The 'timeLimit' must be greater than or equal to 0 (0 means unlimited)");

            if (BoardCount != null && BoardCount < 0)
                errors.Add("The 'boardCount' must be greater than or equal to 0 (0 means unlimited)");

            return errors.ToArray();
        }
    }
}
