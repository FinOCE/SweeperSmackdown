using Newtonsoft.Json;

namespace SweeperSmackdown.Models;

public class Authentication
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("accessToken")]
    public string AccessToken { get; set; }

    [JsonProperty("refreshToken")]
    public string RefreshToken { get; set; }

    [JsonProperty("ttl")]
    public int TimeToLive { get; set; }

    [JsonProperty("scope")]
    public string Scope { get; set; }

    public Authentication(string id, string accessToken, string refreshToken, int timeToLive, string scope)
    {
        Id = id;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        TimeToLive = timeToLive;
        Scope = scope;
    }
}
