using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs;

public class LoginPostRequestDto
{
    [JsonProperty("accessToken")]
    public string AccessToken { get; set; } = null!;

    [JsonProperty("guildId")]
    public string? GuildId { get; set; }

    [JsonProperty("mocked")]
    public bool Mocked { get; set; }
}
