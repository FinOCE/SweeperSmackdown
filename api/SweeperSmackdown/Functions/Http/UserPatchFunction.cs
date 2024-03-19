using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public class UserPatchFunctionPayload
{
}

public static class UserPatchFunction
{
    [FunctionName(nameof(UserPatchFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "lobbies/{lobbyId}/users/{userId}")] UserPatchFunctionPayload payload,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId,
        string userId)
    {
        await Task.Delay(0);
        return new NoContentResult();
    }
}
