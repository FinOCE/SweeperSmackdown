using Newtonsoft.Json;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public IEnumerable<string> UserIds { get; set; }

    [JsonProperty("scores")]
    [JsonPropertyName("scores")]
    public IDictionary<string, int> Scores { get; set; }

    [JsonProperty("wins")]
    [JsonPropertyName("wins")]
    public IDictionary<string, int> Wins { get; set; }

    [JsonProperty("status")]
    [JsonPropertyName("status")]
    public ELobbyStatus Status { get; set; }

    [JsonProperty("stateExpiry")]
    [JsonPropertyName("stateExpiry")]
    public DateTime? StateExpiry { get; set; }

    [JsonProperty("settings")]
    [JsonPropertyName("settings")]
    public GameSettings Settings { get; set; }

    public LobbyResponseDto(
        string lobbyId,
        string hostId,
        IEnumerable<string> userIds,
        IDictionary<string, int> scores,
        IDictionary<string, int> wins,
        ELobbyStatus status,
        DateTime? stateExpiry,
        GameSettings settings)
    {
        LobbyId = lobbyId;
        HostId = hostId;
        UserIds = userIds;
        Scores = scores;
        Wins = wins;
        Status = status;
        StateExpiry = stateExpiry;
        Settings = settings;
    }

    public static LobbyResponseDto FromModel(
        string id,
        LobbyOrchestratorStatus status,
        LobbyStateMachine lobby,
        GameSettingsStateMachine settings)
    {
        return new(
            id,
            lobby.HostId,
            lobby.Players.Where(p => p.Active).Select(p => p.Id),
            lobby.Players.ToDictionary(p => p.Id, p => p.Score),
            lobby.Players.ToDictionary(p => p.Id, p => p.Wins),
            status.Status,
            status.StatusUntil,
            settings.Settings);
    }
}
