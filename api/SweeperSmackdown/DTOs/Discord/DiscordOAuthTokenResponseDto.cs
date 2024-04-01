using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs.Discord;

public class DiscordOAuthTokenResponseDto
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = null!;
}
