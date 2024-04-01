using Microsoft.AspNetCore.Http;
using SweeperSmackdown.Utils;
using System.Linq;

namespace SweeperSmackdown.Extensions;

public static class HttpRequestExtensions
{
    public static string? GetUserId(this HttpRequest req)
    {
        if (!req.Headers.ContainsKey("Authorization"))
            return null;

        var header = req.Headers["Authorization"].First();

        if (!header.StartsWith("Bearer "))
            return null;

        var token = header[7..];

        var parts = token.Split(":");
        var id = parts[0];
        var hash = parts[1];

        if (Hash.Compute(id) != hash)
            return null;

        return id;
    }
}
