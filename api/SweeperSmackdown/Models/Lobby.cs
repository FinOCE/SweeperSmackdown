using Newtonsoft.Json;
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

    [JsonProperty("settings")]
    public GameSettings Settings { get; set; }

    [JsonProperty("state")]
    public ELobbyState State { get; set; }

    public Lobby(
        string id,
        string hostId,
        GameSettings settings)
    {
        Id = id;
        HostId = hostId;
        Settings = settings;
        State = ELobbyState.Init;
    }
}

public enum ELobbyState
{
    Init,
    ConfigureUnlocked,
    ConfigureLocked,
    Play,
    Won,
    Celebrate
}
