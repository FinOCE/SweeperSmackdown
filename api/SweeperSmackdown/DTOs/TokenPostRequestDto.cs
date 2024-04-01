using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs;

public class TokenPostRequestDto
{
    [JsonProperty("code")]
    public string Code { get; set; } = null!;

    [JsonProperty("mocked")]
    public bool Mocked { get; set; }
}
