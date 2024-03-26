using Newtonsoft.Json;
using SweeperSmackdown.Models;

namespace SweeperSmackdown.DTOs;

public class UserResponseDto
{
    [JsonProperty("lobbyId")]
    public string LobbyId { get; set; }

    [JsonProperty("userId")]
    public string UserId { get; set; }

    public UserResponseDto(string lobbyId, string userId)
    {
        LobbyId = lobbyId;
        UserId = userId;
    }

    public static UserResponseDto FromModel(Lobby lobby, string userId) =>
        new(lobby.Id, userId);
}
