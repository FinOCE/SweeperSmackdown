using Newtonsoft.Json;
using SweeperSmackdown.Models;
using System.Linq;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.DTOs;

public class VoteSingleResponseDto
{
    [JsonProperty("lobbyId")]
    [JsonPropertyName("lobbyId")]
    public string LobbyId { get; set; }
    
    [JsonProperty("userId")]
    [JsonPropertyName("userId")]
    public string UserId { get; set; }
    
    [JsonProperty("choice")]
    [JsonPropertyName("choice")]
    public string Choice { get; set; }

    public VoteSingleResponseDto(string lobbyId, string userId, string choice)
    {
        LobbyId = lobbyId;
        UserId = userId;
        Choice = choice;
    }

    public static VoteSingleResponseDto FromModel(Vote vote, string userId) =>
        new(
            vote.LobbyId,
            userId,
            vote.Votes.First(kvp => kvp.Value.Contains(userId)).Key);
}
