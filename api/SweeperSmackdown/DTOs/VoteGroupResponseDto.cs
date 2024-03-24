using Newtonsoft.Json;
using SweeperSmackdown.Models;
using System.Collections.Generic;

namespace SweeperSmackdown.DTOs;

public class VoteGroupResponseDto
{
    [JsonProperty("lobbyId")]
    public string LobbyId { get; }

    [JsonProperty("requiredVotes")]
    public int RequiredVotes { get; }

    [JsonProperty("choices")]
    public string[] Choices { get; }

    [JsonProperty("votes")]
    public IDictionary<string, string[]> Votes { get; }

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
