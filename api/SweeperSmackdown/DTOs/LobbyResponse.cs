using Newtonsoft.Json;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Structures;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.DTOs;

public class LobbyResponse
{
    [JsonProperty("lobbyId")]
    [JsonPropertyName("lobbyId")]
    public string LobbyId { get; set; }

    [JsonProperty("hostId")]
    [JsonPropertyName("hostId")]
    public string HostId { get; set; }

    [JsonProperty("hostManaged")]
    [JsonPropertyName("hostManaged")]
    public bool HostManaged { get; set; }

    [JsonProperty("players")]
    [JsonPropertyName("players")]
    public IEnumerable<Player> Players { get; set; }

    [JsonProperty("status")]
    [JsonPropertyName("status")]
    public PreciseLobbyStatus Status { get; set; }

    [JsonProperty("settings")]
    [JsonPropertyName("settings")]
    public GameSettings Settings { get; set; }

    public LobbyResponse(
        string lobbyId,
        string hostId,
        bool hostManaged,
        IEnumerable<Player> players,
        PreciseLobbyStatus status,
        GameSettings settings)
    {
        LobbyId = lobbyId;
        HostId = hostId;
        HostManaged = hostManaged;
        Players = players;
        Status = status;
        Settings = settings;
    }

    public static LobbyResponse FromModel(
        string id,
        PreciseLobbyStatus preciseLobbyStatus,
        LobbyStateMachine lobby,
        GameSettingsStateMachine settings)
    {
        return new(
            id,
            lobby.HostId,
            lobby.HostManaged,
            lobby.Players,
            preciseLobbyStatus,
            settings.Settings);
    }
}
