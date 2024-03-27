using Newtonsoft.Json;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using System.Collections.Generic;
using System.Linq;

namespace SweeperSmackdown.DTOs;

public class BoardSolutionPostRequestDto
{
    [JsonProperty("gameState")]
    public string GameState { get; set; } = null!;
}
