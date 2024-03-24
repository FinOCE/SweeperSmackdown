using Newtonsoft.Json;
using System.Collections.Generic;

namespace SweeperSmackdown.Models
{
    public class Vote
    {
        [JsonProperty("id")]
        public string LobbyId { get; set; }
        
        [JsonProperty("votes")]
        public IDictionary<string, string[]> Votes { get; set; }

        [JsonProperty("requiredVotes")]
        public int RequiredVotes { get; set; }

        [JsonProperty("choices")]
        public string[] Choices { get; set; }

        public Vote(string lobbyId, IDictionary<string, string[]> votes, int requiredVotes, string[] choices)
        {
            LobbyId = lobbyId;
            Votes = votes;
            RequiredVotes = requiredVotes;
            Choices = choices;
        }
    }
}
