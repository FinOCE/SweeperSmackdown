using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Functions.Activities;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators.Requests.Lobbies;

public class LobbyFetchFunctionProps
{
    public string LobbyId { get; set; }

    public LobbyFetchFunctionProps(string lobbyId)
    {
        LobbyId = lobbyId;
    }
}

public static class LobbyFetchFunction
{
    public static async Task<LobbyResponse?> Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var props = ctx.GetInput<LobbyFetchFunctionProps>();

        return await ctx.CallActivityAsync<LobbyResponse?>(
            nameof(GetLobbyActivityFunction),
            new GetLobbyActivityFunctionProps(props.LobbyId));
    }
}
