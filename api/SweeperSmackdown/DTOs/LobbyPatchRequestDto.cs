using Newtonsoft.Json;

namespace SweeperSmackdown.DTOs;

public class LobbyPatchRequestDto
{
    [JsonProperty("hostId")]
    public string? HostId { get; set; }

    [JsonProperty("hostManaged")]
    public bool? HostManaged { get; set; }
}
