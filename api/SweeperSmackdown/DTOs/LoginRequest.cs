using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs;

public class LoginRequest
{
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; } = null!;

    [JsonProperty("mocked")]
    public bool Mocked { get; set; }
}
