using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators.Requests.Lobbies;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http.Lobbies;

public static class LobbyPutFunction
{
    [FunctionName(nameof(LobbyPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        // Only allow if user is logged in
        var requesterId = req.GetUserId();

        if (requesterId is null)
            return new StatusCodeResult(401);

        // Check if lobby already exists
        var lobby = await entityClient.ReadEntityStateAsync<LobbyStateMachine>(
            Id.For<LobbyStateMachine>(lobbyId));

        if (lobby.EntityExists)
        {
            await orchestrationClient.StartNewAsync(
                nameof(LobbyFetchFunction),
                new LobbyFetchFunctionProps(lobbyId));

            // TODO: Poll orchestration and return 200 result

            return new AcceptedResult();
        }
        else
        {
            await orchestrationClient.StartNewAsync(
                nameof(LobbyCreateFunction),
                new LobbyCreateFunctionProps(lobbyId, requesterId));

            // TODO: Poll orchestration and return 201 result

            return new AcceptedResult();
        }
    }
}
