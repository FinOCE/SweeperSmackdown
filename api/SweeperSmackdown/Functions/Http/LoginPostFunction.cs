using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.DTOs.Discord;
using SweeperSmackdown.Utils;
using System.Net.Http;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public static class LoginPostFunction
{
    [FunctionName(nameof(LoginPostFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "login")] LoginRequest payload)
    {
        string id;
        if (payload.Mocked)
        {
            // Return access token to be used as the ID
            id = payload.AccessToken;

            // TODO: Make this actually secure (this is not !!!)
        }
        else
        {
            // Get user info from Discord
            using HttpClient client = new();
            using HttpRequestMessage req = new(HttpMethod.Get, "https://discord.com/api/users/@me");

            req.Headers.Clear();
            req.Headers.Add("Authorization", $"Bearer {payload.AccessToken}");

            var res = await client.SendAsync(req);
            var data = await res.Content.ReadAsAsync<DiscordOAuthUserResponseDto>();
            
            id = data.Id;
        }

        // Generate token for the given ID
        var hash = Hash.Compute(id);

        // Return the generated token
        return new OkObjectResult(new LoginResponse($"{id}:{hash}"));
    }
}
