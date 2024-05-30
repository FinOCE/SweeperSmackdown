using SweeperSmackdown.Structures;
using System.Threading.Tasks;

namespace SweeperSmackdown.Strategies.Auth;

public interface IAuthStrategy
{
    public Task<string> GenerateAccessToken(string code);

    public Task<User> GetUserInfo(string accessToken, string? guildId);
}
