using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.DTOs.Discord;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public static class TokenPostFunction
{
    [FunctionName(nameof(TokenPostFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "token")] TokenPostRequestDto payload)
    {
        if (payload.Mocked)
        {
            // TODO: Implement secure non-Discord authentication
            
            return new OkObjectResult(new TokenPostResponseDto(payload.Code));
        }
        else
        {
            using HttpClient client = new();
            using FormUrlEncodedContent content = new(new[]
            {
            KeyValuePair.Create("code", payload.Code)
        });

            content.Headers.Clear();
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var res = await client.PostAsync("https://discord.com/api/oauth2/token", content);
            var data = await res.Content.ReadAsAsync<DiscordOAuthTokenResponseDto>();

            return new OkObjectResult(new TokenPostResponseDto(data.AccessToken));
        }
    }
}
