using Newtonsoft.Json;
using SweeperSmackdown.Models;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.DTOs;

public class LobbyUserResponseDto
{
    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonProperty("lobbyId")]
    [JsonPropertyName("lobbyId")]
    public string LobbyId { get; set; }

    [JsonProperty("active")]
    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonProperty("score")]
    [JsonPropertyName("score")]
    public int Score { get; set; }

    [JsonProperty("wins")]
    [JsonPropertyName("wins")]
    public int Wins { get; set; }

    public LobbyUserResponseDto(string id, string lobbyId, bool active, int score, int wins)
    {
        Id = id;
        LobbyId = lobbyId;
        Active = active;
        Score = score;
        Wins = wins;
    }

    public static LobbyUserResponseDto FromModel(Player player) =>
        new(player.Id, player.LobbyId, player.Active, player.Score, player.Wins);
}
