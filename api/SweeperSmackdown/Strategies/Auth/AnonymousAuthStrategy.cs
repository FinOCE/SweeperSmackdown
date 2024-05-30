using SweeperSmackdown.Structures;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SweeperSmackdown.Strategies.Auth;

public class AnonymousAuthStrategy : IAuthStrategy
{
    public Task<string> GenerateAccessToken(string code)
    {
        // TODO: Generate real SECURE access token

        return Task.FromResult(RandomNumberGenerator.GetInt32(1000000).ToString());
    }

    public Task<string> Refresh(string refreshToken)
    {
        // TODO: Refresh real SECURE access token

        return Task.FromResult(refreshToken);
    }

    public Task<User> GetUserInfo(string accessToken, string? guildId)
    {
        // TODO: Return real user info

        string id = accessToken;
        return Task.FromResult(new User(id, id, null));
    }
}
