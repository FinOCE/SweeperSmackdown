using Newtonsoft.Json;
using SweeperSmackdown.Models;
using SweeperSmackdown.Structures;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.DTOs;

public class LobbyResponseDto
{
    [JsonProperty("lobbyId")]
    [JsonPropertyName("lobbyId")]
    public string LobbyId { get; set; }

    [JsonProperty("hostId")]
    [JsonPropertyName("hostId")]
    public string HostId { get; set; }
    
    [JsonProperty("userIds")]
    [JsonPropertyName("userIds")]
    public string[] UserIds { get; set; }

    [JsonProperty("scores")]
    [JsonPropertyName("scores")]
    public IDictionary<string, int> Scores { get; set; }

    [JsonProperty("wins")]
    [JsonPropertyName("wins")]
    public IDictionary<string, int> Wins { get; set; }

    [JsonProperty("settings")]
    [JsonPropertyName("settings")]
    public GameSettings Settings { get; set; }

    public LobbyResponseDto(
        string lobbyId,
        string hostId,
        string[] userIds,
        IDictionary<string, int> scores,
        IDictionary<string, int> wins,
        GameSettings settings)
    {
        LobbyId = lobbyId;
        HostId = hostId;
        UserIds = userIds;
        Scores = scores;
        Wins = wins;
        Settings = settings;
    }

    public static LobbyResponseDto FromModel(Lobby lobby) =>
        new(
            lobby.Id,
            lobby.HostId,
            lobby.UserIds,
            lobby.Scores,
            lobby.Wins,
            lobby.Settings);
}
