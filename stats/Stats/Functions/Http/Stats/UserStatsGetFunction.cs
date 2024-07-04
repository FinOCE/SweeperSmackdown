using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SweeperSmackdown.Stats.Assets;
using SweeperSmackdown.Stats.Models;
using System.Net;
using System.Threading.Tasks;
using SweeperSmackdown.Stats.DTOs;

namespace SweeperSmackdown.Stats.Functions.Http.Stats;

public static class UserStatsGetFunction
{
    [Function(nameof(UserStatsGetFunction))]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "get",
            Route = "users/{userId}/stats")]
        HttpRequestData req,
        [CosmosDBInput(
            Constants.DATABASE_NAME,
            Constants.STATS_CONTAINER_NAME,
            Id = "{userId}",
            PartitionKey = "{userId}")]
        PlayerInfo? playerInfo,
        string userId)
    {
        if (playerInfo is null)
            return req.CreateResponse(HttpStatusCode.NotFound);

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(StatResponse.FromModel(playerInfo));

        return res;
    }
}
