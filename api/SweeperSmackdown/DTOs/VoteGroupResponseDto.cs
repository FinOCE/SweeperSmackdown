using Newtonsoft.Json;
using SweeperSmackdown.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.DTOs;

public class VoteGroupResponseDto
{
    [JsonProperty("lobbyId")]
    [JsonPropertyName("lobbyId")]
    public string LobbyId { get; set; }

    [JsonProperty("requiredVotes")]
    [JsonPropertyName("requiredVotes")]
    public int RequiredVotes { get; set; }

    [JsonProperty("choices")]
    [JsonPropertyName("choices")]
    public string[] Choices { get; set; }

    [JsonProperty("votes")]
    [JsonPropertyName("votes")]
    public IDictionary<string, string[]> Votes { get; set; }

    public VoteGroupResponseDto(string lobbyId, int requiredVotes, string[] choices, IDictionary<string, string[]> votes)
    {
        LobbyId = lobbyId;
        RequiredVotes = requiredVotes;
        Choices = choices;
        Votes = votes;
    }

    public static VoteGroupResponseDto FromModel(Vote vote) =>
        new(
            vote.LobbyId,
            vote.RequiredVotes,
            vote.Choices,
            vote.Votes);
}
