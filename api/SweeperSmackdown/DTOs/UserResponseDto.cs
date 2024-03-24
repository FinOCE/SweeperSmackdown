using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs;

public class UserResponseDto
{
    [JsonProperty("lobbyId")]
    public string LobbyId { get; }

    [JsonProperty("userId")]
    public string UserId { get; }

    public UserResponseDto(string lobbyId, string userId)
    {
        LobbyId = lobbyId;
        UserId = userId;
    }
}
