using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators.Requests.Lobbies;

public class LobbyUpdateFunctionProps
{
    public string LobbyId { get; set; }

    public string? RequesterId { get; set; }

    public string? HostId { get; set; }

    public bool? HostManaged { get; set; }

    public LobbyUpdateFunctionProps(string lobbyId, string? requesterId, string? hostId, bool? hostManaged)
    {
        LobbyId = lobbyId;
        RequesterId = requesterId;
        HostId = hostId;
        HostManaged = hostManaged;
    }
}

public static class LobbyUpdateFunction
{
    [FunctionName(nameof(LobbyUpdateFunction))]
    public static async Task<LobbyResponse> Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var props = ctx.GetInput<LobbyUpdateFunctionProps>();

        var hostId = await ctx.CallEntityAsync<string>(
            Id.For<LobbyStateMachine>(props.LobbyId),
            nameof(ILobbyStateMachine.GetHost));

        // TODO: Check how this should handle not existing (throw ArgumentException)

        if (props.RequesterId is not null && props.RequesterId != hostId)
            throw new AccessViolationException();

        // Update to provided values
        if (props.HostId is not null)
            await ctx.CallEntityAsync(
                Id.For<LobbyStateMachine>(props.LobbyId),
                nameof(ILobbyStateMachine.SetHost),
                props.HostId);

        if (props.HostManaged is not null)
            await ctx.CallEntityAsync(
                Id.For<LobbyStateMachine>(props.LobbyId),
                nameof(ILobbyStateMachine.SetHostManaged),
                props.HostManaged);

        var res = await ctx.CallActivityAsync<LobbyResponse?>(
            nameof(GetLobbyActivityFunction),
            new GetLobbyActivityFunctionProps(props.LobbyId));

        return res is null ? throw new ApplicationException() : res;
    }
}
