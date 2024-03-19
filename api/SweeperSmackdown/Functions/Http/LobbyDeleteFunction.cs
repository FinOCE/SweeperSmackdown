using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SweeperSmackdown.Functions.Http;

public static class LobbyDeleteFunction
{
    [FunctionName(nameof(LobbyDeleteFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "lobbies/{lobbyId}")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        // TODO: Figure out best approach for deleting lobbies (if it should even be done explicitly at all)

        await Task.Delay(0);
        return new NoContentResult();
    }
}
