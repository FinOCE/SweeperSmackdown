using SweeperSmackdown.Functions.Entities;
using System.Linq;

namespace SweeperSmackdown.Extensions;

public static class LobbyStateMachineExtensions
{
    public static bool IsAllowedToModifySettings(this LobbyStateMachine lobby, string requesterId)
    {
        if (!lobby.Players.Any(p => p.Id == requesterId))
            return false;

        if (lobby.HostId != requesterId && lobby.HostManaged)
            return false;

        return true;
    }
}
