using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SweeperSmackdown.Stats.Assets;
using SweeperSmackdown.Stats.DTOs;
using SweeperSmackdown.Stats.Models;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SweeperSmackdown.Stats.Functions.Http.Achievements;

public static class UserAchievementGetAllFunction
{
    [Function(nameof(UserAchievementGetAllFunction))]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "users/{userId}/achievements")]
        HttpRequestData req,
        [CosmosDBInput(
            Constants.DATABASE_NAME,
            Constants.ACHIEVEMENTS_CONTAINER_NAME,
            Id = "{userId}",
            PartitionKey = "{userId}")]
        AchievementProgressCollection? progress,
        string userId)
    {
        if (progress is null)
            return req.CreateResponse(HttpStatusCode.NotFound);

        var achievements = Constants.Achievements
            .Select(a => (Achievement: a, Progress: progress.Achievements.FirstOrDefault(ac => ac.Id == a.Id)))
            .Select(a => UserAchievementResponse.FromModel(a.Achievement, a.Progress));

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(achievements);

        return res;
    }
}
