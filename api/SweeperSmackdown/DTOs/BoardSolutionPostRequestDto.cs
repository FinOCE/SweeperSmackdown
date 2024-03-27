using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs;

public class BoardSolutionPostRequestDto
{
    [JsonProperty("gameState")]
    public string GameState { get; set; } = null!;
}
