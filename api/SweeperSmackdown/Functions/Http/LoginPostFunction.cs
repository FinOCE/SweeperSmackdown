using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Strategies.Auth;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public static class LoginPostFunction
{
    [FunctionName(nameof(LoginPostFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "login")] LoginPostRequestDto payload)
    {
        var type = payload.Mocked
            ? "anonymous"
            : "discord";

        var authStrategy = AuthStrategyProvider.GetStrategy(type);
        var user = await authStrategy.GetUserInfo(payload.AccessToken);

        var hash = Hash.Compute(user.Id);
        var bearerToken = $"{user.Id}:{hash}";

        return new OkObjectResult(new LoginPostResponseDto(bearerToken));
    }
}
