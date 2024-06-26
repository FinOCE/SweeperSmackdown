﻿using Newtonsoft.Json;
using SweeperSmackdown.Models;
using SweeperSmackdown.Structures;
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

    [JsonProperty("state")]
    [JsonPropertyName("state")]
    public ELobbyState State { get; set; }

    [JsonProperty("settings")]
    [JsonPropertyName("settings")]
    public GameSettings Settings { get; set; }

    public LobbyResponseDto(
        string lobbyId,
        string hostId,
        IEnumerable<string> userIds,
        IDictionary<string, int> scores,
        IDictionary<string, int> wins,
        ELobbyState state,
        GameSettings settings)
    {
        LobbyId = lobbyId;
        HostId = hostId;
        UserIds = userIds;
        Scores = scores;
        Wins = wins;
        State = state;
        Settings = settings;
    }

    public static LobbyResponseDto FromModel(Lobby lobby, IEnumerable<Player> players)
    {
        return new(
            lobby.Id,
            lobby.HostId,
            players.Where(p => p.Active).Select(p => p.Id),
            players.ToDictionary(p => p.Id, p => p.Score),
            players.ToDictionary(p => p.Id, p => p.Wins),
            lobby.State,
            lobby.Settings);
    }
}
