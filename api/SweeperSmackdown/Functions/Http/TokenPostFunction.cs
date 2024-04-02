using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.DTOs.Discord;
using SweeperSmackdown.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SweeperSmackdown.Functions.Http;

public static class TokenPostFunction
{
    [FunctionName(nameof(TokenPostFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "token")] TokenPostRequestDto payload)
    {
        if (payload.Mocked)
        {
            // Generate random number to use as non-Discord token/id
            return new OkObjectResult(
                new TokenPostResponseDto(
                    RandomNumberGenerator.GetInt32(1000000).ToString()));
        }
        else
        {
            // Get access token from Discord
            using HttpClient client = new();

            var body = new Dictionary<string, string>
            {
                { "client_id", Environment.GetEnvironmentVariable("DiscordClientId")! },
                { "client_secret", Environment.GetEnvironmentVariable("DiscordClientSecret")! },
                { "grant_type", "authorization_code" },
                { "code", payload.Code }
            };
            var encoded = string.Join("&", body.Select(kvp => $"{kvp.Key}={kvp.Value}"));

            var content = new StringContent(encoded);
            content.Headers.Clear();
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            var res = await client.PostAsync("https://discord.com/api/oauth2/token", content);
            var data = await res.Content.ReadAsAsync<DiscordOAuthTokenResponseDto>();
            
            return new OkObjectResult(new TokenPostResponseDto(data.AccessToken));
        }
    }
}
