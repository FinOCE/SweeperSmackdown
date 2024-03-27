using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs;

public class VoteDeleteRequestDto
{
    [JsonProperty("force")]
    public bool? Force { get; set; }
}
