using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.DTOs;

public class TokenPostResponseDto
{
    [JsonProperty("accessToken")]
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }

    public TokenPostResponseDto(string accessToken)
    {
        AccessToken = accessToken;
    }
}
