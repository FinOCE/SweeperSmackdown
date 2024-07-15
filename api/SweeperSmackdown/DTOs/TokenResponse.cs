using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.DTOs;

public class TokenResponse
{
    [JsonProperty("accessToken")]
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }

    public TokenResponse(string accessToken)
    {
        AccessToken = accessToken;
    }
}
