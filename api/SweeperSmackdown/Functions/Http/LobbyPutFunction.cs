using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public class LobbyPutFunctionPayload
{
}

public static class LobbyPutFunction
{
    [FunctionName(nameof(LobbyPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}")] LobbyPutFunctionPayload payload,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        string lobbyId)
    {
        await Task.Delay(0);
        return new NoContentResult();
    }
}
