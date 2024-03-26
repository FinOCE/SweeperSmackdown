using Newtonsoft.Json;
using SweeperSmackdown.Models;
using System.Collections.Generic;

namespace SweeperSmackdown.DTOs;

public class VoteGroupResponseDto
{
    [JsonProperty("lobbyId")]
    public string LobbyId { get; set; }

    [JsonProperty("requiredVotes")]
    public int RequiredVotes { get; set; }

    [JsonProperty("choices")]
    public string[] Choices { get; set; }

    [JsonProperty("votes")]
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
