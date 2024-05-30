using Microsoft.AspNetCore.Http;
using SweeperSmackdown.Utils;
using System.Linq;

namespace SweeperSmackdown.Extensions;

public static class HttpRequestExtensions
{
    public static string? GetUserId(this HttpRequest req)
    {
        // Get authorization header
        if (!req.Headers.ContainsKey("Authorization"))
            return null;

        var header = req.Headers["Authorization"].First();

        // Ensure it is a bearer token
        if (!header.StartsWith("Bearer "))
            return null;

        var token = header[7..];

        //Extract parts of token
        var parts = token.Split(":");

        if (parts.Length != 2)
            return null;
        
        var id = parts[0];
        var hash = parts[1];

        // Verify token
        if (!Crypto.VerifyHash(id, hash))
            return null;

        // Return ID on valid request
        return id;
    }
}
