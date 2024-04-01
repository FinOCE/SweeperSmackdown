using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.DTOs;

public class TokenPostResponseDto
{
    [JsonProperty("access_token")]
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    public TokenPostResponseDto(string accessToken)
    {
        AccessToken = accessToken;
    }
}
