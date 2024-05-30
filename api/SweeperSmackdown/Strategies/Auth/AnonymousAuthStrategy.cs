using SweeperSmackdown.Structures;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SweeperSmackdown.Strategies.Auth;

public class AnonymousAuthStrategy : IAuthStrategy
{
    public Task<string> GenerateAccessToken(string code)
    {
        // TODO: Generate real SECURE bearer token

        return Task.FromResult(RandomNumberGenerator.GetInt32(1000000).ToString());
    }

    public Task<User> GetUserInfo(string accessToken, string? guildId)
    {
        // TODO: Return real user info

        string id = accessToken;
        return Task.FromResult(new User(id, id, null));
    }
}
