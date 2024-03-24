using Newtonsoft.Json;
using SweeperSmackdown.Structures;
using System.Collections.Generic;

namespace SweeperSmackdown.Models;

public class Lobby
{
    [JsonProperty("id")]
    public string Id { get; set; }

    public string HostId { get; set; }
    
    [JsonProperty("userIds")]
    public string[] UserIds { get; set; }

    [JsonProperty("wins")]
    public IDictionary<string, int> Wins { get; set; }

    [JsonProperty("settings")]
    public GameSettings Settings { get; set; }

    public Lobby(
        string id,
        string hostId,
        string[] userIds,
        IDictionary<string, int> wins,
        GameSettings settings)
    {
        Id = id;
        HostId = hostId;
        UserIds = userIds;
        Wins = wins;
        Settings = settings;
    }
}
