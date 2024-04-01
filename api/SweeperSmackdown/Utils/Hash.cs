using System.Security.Cryptography;
using System.Text;
using System;

namespace SweeperSmackdown.Utils;

public static class Hash
{
    public static string Compute(string value)
    {
        var key = Environment.GetEnvironmentVariable("BearerTokenSecretKey")!;
        using var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(hashBytes);
    }

    public static bool Verify(string value, string hash)
    {
        var computed = Compute(value);
        return computed == hash;
    }
}
