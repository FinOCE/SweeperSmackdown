using Newtonsoft.Json;

namespace SweeperSmackdown.Models;

/// <summary>
/// The representation of a player in a lobby.
/// </summary>
public class Player
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("lobbyId")]
    public string LobbyId { get; set; }

    [JsonProperty("active")]
    public bool Active { get; set; }

    [JsonProperty("score")]
    public int Score { get; set; }

    [JsonProperty("wins")]
    public int Wins { get; set; }

    public Player(string id, string lobbyId, bool active, int score, int wins)
    {
        Id = id;
        LobbyId = lobbyId;
        Active = active;
        Score = score;
        Wins = wins;
    }
}
