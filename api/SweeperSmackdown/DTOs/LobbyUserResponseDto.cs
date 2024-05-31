using Newtonsoft.Json;
using SweeperSmackdown.Models;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.DTOs;

public class LobbyUserResponseDto
{
    [JsonProperty("lobbyId")]
    [JsonPropertyName("lobbyId")]
    public string LobbyId { get; set; }

    [JsonProperty("userId")]
    [JsonPropertyName("userId")]
    public string UserId { get; set; }

    public LobbyUserResponseDto(string lobbyId, string userId)
    {
        LobbyId = lobbyId;
        UserId = userId;
    }

    public static LobbyUserResponseDto FromModel(Lobby lobby, string userId) =>
        new(lobby.Id, userId);
}
