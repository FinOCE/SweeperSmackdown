using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs;

public class VotePutRequestDto
{
    [JsonProperty("choice")]
    public string Choice { get; set; } = null!;
}
