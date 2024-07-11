using SweeperSmackdown.Functions.Entities;
using System.Linq;

namespace SweeperSmackdown.Extensions;

public static class GameSettingsStateMachineExtensions
{
    public static bool IsConfigurable(this GameSettingsStateMachine settings)
    {
        return GameSettingsStateMachine.ValidStatesToUpdateSettings.Contains(settings.State);
    }
}
