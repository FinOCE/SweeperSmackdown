using Newtonsoft.Json;
using SweeperSmackdown.Models;
using SweeperSmackdown.Structures;
using System.Collections.Generic;

namespace SweeperSmackdown.DTOs;

public class LobbyResponseDto
{
    [JsonProperty("lobbyId")]
    public string LobbyId { get; }

    [JsonProperty("hostId")]
    public string HostId { get; }
    
    [JsonProperty("userIds")]
    public string[] UserIds { get; }
    
    [JsonProperty("wins")]
    public IDictionary<string, int> Wins { get; }

    [JsonProperty("settings")]
    public GameSettings Settings { get; }

    public LobbyResponseDto(string lobbyId, string hostId, string[] userIds, IDictionary<string, int> wins, GameSettings settings)
    {
        LobbyId = lobbyId;
        HostId = hostId;
        UserIds = userIds;
        Wins = wins;
        Settings = settings;
    }

    public static LobbyResponseDto FromModel(Lobby lobby) =>
        new(
            lobby.Id,
            lobby.HostId,
            lobby.UserIds,
            lobby.Wins,
            lobby.Settings);
}
