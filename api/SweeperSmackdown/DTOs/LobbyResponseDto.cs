using Newtonsoft.Json;
using SweeperSmackdown.Structures;
using System.Collections.Generic;

namespace SweeperSmackdown.DTOs;

public class LobbyResponseDto
{
    [JsonProperty("lobbyId")]
    public string LobbyId { get; }
    
    [JsonProperty("userIds")]
    public string[] UserIds { get; }
    
    [JsonProperty("wins")]
    public IDictionary<string, int> Wins { get; }

    [JsonProperty("settings")]
    public GameSettings Settings { get; }

    public LobbyResponseDto(string lobbyId, string[] userIds, IDictionary<string, int> wins, GameSettings settings)
    {
        LobbyId = lobbyId;
        UserIds = userIds;
        Wins = wins;
        Settings = settings;
    }
}
