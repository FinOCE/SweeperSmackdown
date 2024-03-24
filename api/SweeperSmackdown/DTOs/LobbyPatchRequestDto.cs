using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs;

public class LobbyPatchRequestDto
{
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
}
