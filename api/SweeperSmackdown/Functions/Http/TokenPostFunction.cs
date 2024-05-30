using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Strategies.Auth;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public static class TokenPostFunction
{
    [FunctionName(nameof(TokenPostFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "token")] TokenPostRequestDto payload)
    {
        var type = payload.Mocked
            ? "anonymous"
            : "discord";

        var authStrategy = AuthStrategyProvider.GetStrategy(type);
        var accessToken = await authStrategy.GenerateAccessToken(payload.Code);

        return new OkObjectResult(new TokenPostResponseDto(accessToken));
    }
}
