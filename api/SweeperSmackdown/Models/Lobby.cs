using Newtonsoft.Json;
using SweeperSmackdown.Structures;
using System;
using System.Collections.Generic;

namespace SweeperSmackdown.Models;

public class Lobby
{
    [JsonProperty("id")]
    public string Id { get; set; }

    public string HostId { get; set; }
    
    [JsonProperty("userIds")]
    public string[] UserIds { get; set; }

    [JsonProperty("addedUserIds")]
    public string[] AddedUserIds { get; set; }

    [JsonProperty("scores")]
    public IDictionary<string, int> Scores { get; set; }

    [JsonProperty("wins")]
    public IDictionary<string, int> Wins { get; set; }

    [JsonProperty("settings")]
    public GameSettings Settings { get; set; }

    public Lobby(
        string id,
        string hostId,
        string[] userIds,
        IDictionary<string, int> scores,
        IDictionary<string, int> wins,
        GameSettings settings)
    {
        Id = id;
        HostId = hostId;
        UserIds = userIds;
        AddedUserIds = Array.Empty<string>();
        Scores = scores;
        Wins = wins;
        Settings = settings;
    }
}
