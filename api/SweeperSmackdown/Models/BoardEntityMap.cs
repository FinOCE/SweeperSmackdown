using Newtonsoft.Json;

namespace SweeperSmackdown.Models;

public class BoardEntityMap
{
    [JsonProperty("id")]
    public string LobbyId { get; set; }

    [JsonProperty("boards")]
    public string[] BoardIds { get; set; }
    
    public BoardEntityMap(string lobbyId, string[] boardIds)
    {
        LobbyId = lobbyId;
        BoardIds = boardIds;
    }
}
