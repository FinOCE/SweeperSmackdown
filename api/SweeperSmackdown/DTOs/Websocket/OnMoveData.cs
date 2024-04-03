using System.Text.Json.Serialization;

namespace SweeperSmackdown.DTOs.Websocket;

public class OnMoveData
{
    [JsonPropertyName("lobbyId")]
    public string LobbyId { get; set; } = null!;
    
    [JsonPropertyName("reveals")]
    public int[]? Reveals { get; set; }

    [JsonPropertyName("flagAdd")]
    public int? FlagAdd { get; set; }

    [JsonPropertyName("flagRemove")]
    public int? FlagRemove { get; set; }
}
