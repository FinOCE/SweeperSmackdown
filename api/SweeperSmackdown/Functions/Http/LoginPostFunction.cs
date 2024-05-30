using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Models;
using SweeperSmackdown.Strategies.Auth;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public static class LoginPostFunction
{
    [FunctionName(nameof(LoginPostFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "login")] LoginPostRequestDto payload,
        [CosmosDB(
            containerName: DatabaseConstants.USER_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<User> db)
    {
        var type = payload.Mocked
            ? "anonymous"
            : "discord";

        var authStrategy = AuthStrategyProvider.GetStrategy(type);
        var user = await authStrategy.GetUserInfo(payload.AccessToken, payload.GuildId);

        var hash = Crypto.ComputeHash(user.Id);
        var bearerToken = $"{user.Id}:{hash}";

        await db.AddAsync(user);

        return new OkObjectResult(new LoginPostResponseDto(bearerToken));
    }
}
