using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Models;
using SweeperSmackdown.Strategies.Auth;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public static class TokenPostFunction
{
    [FunctionName(nameof(TokenPostFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "token")] TokenPostRequestDto payload,
        [CosmosDB(
            containerName: DatabaseConstants.AUTH_CONTAINER_NAME,
            databaseName: DatabaseConstants.DATABASE_NAME,
            Connection = "CosmosDbConnectionString")]
            IAsyncCollector<Authentication> db)
    {
        var type = payload.Mocked
            ? "anonymous"
            : "discord";

        var authStrategy = AuthStrategyProvider.GetStrategy(type);
        var auth = await authStrategy.Authenticate(payload.Code);

        await db.AddAsync(auth);

        return new OkObjectResult(new TokenPostResponseDto(auth.AccessToken));
    }
}
