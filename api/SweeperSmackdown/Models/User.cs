using Newtonsoft.Json;

namespace SweeperSmackdown.Models;

public class User
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    [JsonProperty("avatarUrl")]
    public string? AvatarUrl { get; set; }

    public User(string id, string displayName, string? avatarUrl)
    {
        Id = id;
        DisplayName = displayName;
        AvatarUrl = avatarUrl;
    }
}
