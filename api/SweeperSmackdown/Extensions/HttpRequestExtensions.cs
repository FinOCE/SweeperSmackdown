using Microsoft.AspNetCore.Http;
using System.Linq;

namespace SweeperSmackdown.Extensions;

public static class HttpRequestExtensions
{
    public static string? GetUserId(this HttpRequest req)
    {
        // TODO: Properly validate to get user ID

        if (!req.Headers.ContainsKey("Authorization"))
            return null;

        var header = req.Headers["Authorization"].First();

        if (!header.StartsWith("Bearer "))
            return null;

        var token = header[7..];

        if (token.Split('.').Length == 3)
        {
            // Discord token
            return token;
            
            // TODO: Properly parse Discord token into user ID
        }
        else
        {
            // Non-Discord token
            return token;

            // TODO: Properly parse non-Discord token into user ID
        }
    }
}
