using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs;

public class LobbyPatchRequest
{
    [JsonProperty("hostId")]
    public string? HostId { get; set; }

    [JsonProperty("hostManaged")]
    public bool? HostManaged { get; set; }
}
