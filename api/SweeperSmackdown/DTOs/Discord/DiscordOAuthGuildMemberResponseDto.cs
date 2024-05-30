using Newtonsoft.Json;
using System;

namespace SweeperSmackdown.DTOs.Discord;

public class DiscordOAuthGuildMemberResponseDto
{
    [JsonProperty("user")]
    public DiscordOAuthUserResponseDto User { get; set; } = null!;

    [JsonProperty("nick")]
    public string? Nickname { get; set; }

    [JsonProperty("avatar")]
    public string? Avatar { get; set; }

    [JsonProperty("roles")]
    public string[] Roles { get; set; } = new string[0];

    [JsonProperty("joined_at")]
    public DateTime JoinedAt { get; set; }

    [JsonProperty("premium_since")]
    public DateTime PremiumSince { get; set; }

    [JsonProperty("deaf")]
    public bool Deafened { get; set; }

    [JsonProperty("mute")]
    public bool Muted { get; set; }

    [JsonProperty("flags")]
    public int Flags { get; set; }

    [JsonProperty("pending")]
    public bool? Pending { get; set; }

    [JsonProperty("permissions")]
    public string? Permissions { get; set; }

    [JsonProperty("communication_disabled_until")]
    public DateTime? CommunicationDisabledUntil { get; set; }

    [JsonProperty("avatar_decoration_data")]
    public AvatarDecorationData? AvatarDecorationData { get; set; }
}
