using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.DTOs;

public class LoginResponse
{
    [JsonProperty("bearerToken")]
    [JsonPropertyName("bearerToken")]
    public string BearerToken { get; set; }

    public LoginResponse(string bearerToken)
    {
        BearerToken = bearerToken;
    }
}
