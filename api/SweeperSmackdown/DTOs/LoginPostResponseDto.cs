using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.DTOs;

public class LoginPostResponseDto
{
    [JsonProperty("bearerToken")]
    [JsonPropertyName("bearerToken")]
    public string BearerToken { get; set; }

    public LoginPostResponseDto(string bearerToken)
    {
        BearerToken = bearerToken;
    }
}
