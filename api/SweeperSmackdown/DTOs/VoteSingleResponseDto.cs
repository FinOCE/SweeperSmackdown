using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs;

public class VoteSingleResponseDto
{
    [JsonProperty("lobbyId")]
    public string LobbyId { get; }
    
    [JsonProperty("userId")]
    public string UserId { get; }
    
    [JsonProperty("choice")]
    public string Choice { get; }

    public VoteSingleResponseDto(string lobbyId, string userId, string choice)
    {
        LobbyId = lobbyId;
        UserId = userId;
        Choice = choice;
    }
}
