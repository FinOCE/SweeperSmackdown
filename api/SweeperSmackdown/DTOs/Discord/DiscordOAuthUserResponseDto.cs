using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs.Discord;

public class DiscordOAuthUserResponseDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = null!;

    // https://discord.com/developers/docs/resources/user#user-object-user-structure
}
