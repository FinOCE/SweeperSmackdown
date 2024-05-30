using SweeperSmackdown.Models;
using System.Threading.Tasks;

namespace SweeperSmackdown.Strategies.Auth;

public interface IAuthStrategy
{
    public Task<Authentication> Authenticate(string code);

    public Task<Authentication> Refresh(string id, string refreshToken);

    public Task<User> GetUserInfo(string accessToken, string? guildId);
}
