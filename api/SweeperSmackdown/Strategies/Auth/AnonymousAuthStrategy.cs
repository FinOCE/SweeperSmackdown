using Azure.Core;
using SweeperSmackdown.Models;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SweeperSmackdown.Strategies.Auth;

public class AnonymousAuthStrategy : IAuthStrategy
{
    public async Task<Authentication> Authenticate(string code)
    {
        // TODO: Generate real SECURE access token

        var accessToken = RandomNumberGenerator.GetInt32(1000000).ToString();
        var refreshToken = accessToken;

        var user = await GetUserInfo(accessToken, null);

        return new Authentication(user.Id, accessToken, refreshToken, 86400 * 14, "");
    }

    public Task<Authentication> Refresh(string id, string refreshToken)
    {
        // TODO: Refresh real SECURE access token

        var accessToken = refreshToken;

        return Task.FromResult(new Authentication(id, accessToken, refreshToken, 86400 * 14, ""));
    }

    public Task<User> GetUserInfo(string accessToken, string? guildId)
    {
        // TODO: Return real user info

        string id = accessToken;
        return Task.FromResult(new User(id, id, null));
    }
}
