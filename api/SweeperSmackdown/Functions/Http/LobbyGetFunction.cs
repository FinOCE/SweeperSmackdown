using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SweeperSmackdown.Functions.Http;

public static class LobbyGetFunction
{
    [FunctionName(nameof(LobbyGetFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "lobbies/{lobbyId}")] HttpRequest req,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        await Task.Delay(0);
        return new NoContentResult();
    }
}
