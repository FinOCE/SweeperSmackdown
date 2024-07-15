using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs;

public class BoardSolutionRequest
{
    [JsonProperty("gameState")]
    public string GameState { get; set; } = null!;
}
