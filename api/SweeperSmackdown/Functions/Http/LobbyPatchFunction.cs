using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public class LobbyPatchFunctionPayload
{
    [JsonProperty("lifetime")]
    public int? Lifetime { get; }

    [JsonProperty("mode")]
    public int? Mode { get; }

    [JsonProperty("height")]
    public int? Height { get; }

    [JsonProperty("width")]
    public int? Width { get; }
}

public static class LobbyPatchFunction
{
    [FunctionName(nameof(LobbyPatchFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "lobbies/{lobbyId}")] LobbyPatchFunctionPayload payload,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        await Task.Delay(0);
        return new NoContentResult();
    }
}
