using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public class UserPutFunctionPayload
{
}

public static class UserPutFunction
{
    [FunctionName(nameof(UserPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}/users/{userId}")] UserPutFunctionPayload payload,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId,
        string userId)
    {
        await Task.Delay(0);
        return new NoContentResult();
    }
}
