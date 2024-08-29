using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class CheckLobbyIdAvailableActivityFunctionProps
{
    public string LobbyId { get; set; }

    public CheckLobbyIdAvailableActivityFunctionProps(string lobbyId)
    {
        LobbyId = lobbyId;
    }
}

public static class CheckLobbyIdAvailableActivityFunction
{
    [FunctionName(nameof(CheckLobbyIdAvailableActivityFunction))]
    public static async Task<bool> Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableEntityClient entityClient)
    {
        var props = ctx.GetInput<CheckLobbyIdAvailableActivityFunctionProps>();

        var lobby = await entityClient.ReadEntityStateAsync<LobbyStateMachine>(
                Id.For<LobbyStateMachine>(props.LobbyId));

        return !lobby.EntityExists;
    }
}
