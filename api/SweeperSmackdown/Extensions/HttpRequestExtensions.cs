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
        
        return header[7..];
    }
}
