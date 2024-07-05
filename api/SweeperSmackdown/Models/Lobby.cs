﻿using Newtonsoft.Json;
using System;
using SweeperSmackdown.Structures;

namespace SweeperSmackdown.Models;

/// <summary>
/// The representation of an game lobby.
/// </summary>
public class Lobby
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("hostId")]
    public string HostId { get; set; }

    [JsonProperty("hostManaged")]
    public bool HostManaged { get; set; }

    [JsonProperty("settings")]
    public GameSettings Settings { get; set; }

    [JsonProperty("state")]
    public ELobbyState State { get; set; }

    [JsonProperty("stateExpiry")]
    public DateTime? StateExpiry { get; set; }

    [JsonProperty("_ts")]
    public long UpdatedAt { get; set; }

    public Lobby(
        string id,
        string hostId,
        GameSettings settings)
    {
        Id = id;
        HostId = hostId;
        HostManaged = false;
        Settings = settings;
        State = ELobbyState.ConfigureUnlocked;
        StateExpiry = null;
    }
}

public enum ELobbyState
{
    ConfigureUnlocked,
    ConfigureLocked,
    ConfigureCountdown,
    Play,
    Won,
    Celebrate
}
