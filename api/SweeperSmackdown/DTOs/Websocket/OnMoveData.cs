using System.Text.Json.Serialization;

namespace SweeperSmackdown.DTOs.Websocket;

public class OnMoveData
{
    [JsonPropertyName("reveals")]
    public int[]? Reveals { get; set; }

    [JsonPropertyName("flag")]
    public int? Flag { get; set; }
}
