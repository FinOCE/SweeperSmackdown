using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Collections.Generic;
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

        var hostId = ctx.CallEntityAsync<string>(
            Id.For<LobbyStateMachine>(props.LobbyId),
            nameof(ILobbyStateMachine.GetHost));

        var hostManaged = ctx.CallEntityAsync<bool>(
            Id.For<LobbyStateMachine>(props.LobbyId),
            nameof(ILobbyStateMachine.GetHostManaged));

        var players = ctx.CallEntityAsync<IEnumerable<Player>>(
            Id.For<LobbyStateMachine>(props.LobbyId),
            nameof(ILobbyStateMachine.GetPlayers));

        var status = ctx.CallActivityAsync<PreciseLobbyStatus?>(
            nameof(GetLobbyStatusActivityFunctionProps),
            new GetLobbyStatusActivityFunctionProps(props.LobbyId));

        var settings = ctx.CallEntityAsync<GameSettings>(
            Id.For<GameSettingsStateMachine>(props.LobbyId),
            nameof(IGameSettingsStateMachine.GetSettings));

        // TODO: Check how these should handle not existing

        await Task.WhenAll(hostId, hostManaged, players, status, settings);

        if (status.Result is null)
            return null;

        return new LobbyResponse(
            props.LobbyId,
            hostId.Result,
            hostManaged.Result,
            players.Result,
            status.Result,
            settings.Result);

        // TODO: Probably turn the core of this into an activity
    }
}
