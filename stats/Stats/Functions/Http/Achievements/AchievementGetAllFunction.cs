using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SweeperSmackdown.Stats.Assets;
using SweeperSmackdown.Stats.DTOs;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SweeperSmackdown.Stats.Functions.Http.Achievements;

public static class AchievementGetAllFunction
{
    [Function(nameof(AchievementGetAllFunction))]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "achievements")] HttpRequestData req)
    {
        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(Constants.Achievements.Select(AchievementResponse.FromModel));

        return res;
    }
}
