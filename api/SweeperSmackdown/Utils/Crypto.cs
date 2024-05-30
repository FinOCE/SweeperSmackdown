using System;
using System.Security.Cryptography;
using System.Text;

namespace SweeperSmackdown.Utils;

public static class Crypto
{
    public static string ComputeHash(string value)
    {
        var key = Environment.GetEnvironmentVariable("BearerTokenSecretKey")!;
        using var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyHash(string value, string hash)
    {
        var computed = ComputeHash(value);
        return computed == hash;
    }
}
