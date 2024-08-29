using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators.Requests.Lobbies;

public class LobbyDisposeFunctionProps
{
    public string LobbyId { get; set; }

    public string? RequesterId { get; set; }

    public LobbyDisposeFunctionProps(string lobbyId, string? requesterId)
    {
        LobbyId = lobbyId;
        RequesterId = requesterId;
    }
}

public static class LobbyDisposeFunction
{
    [FunctionName(nameof(LobbyDisposeFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var props = ctx.GetInput<LobbyDisposeFunctionProps>();

        var hostId = await ctx.CallEntityAsync<string>(
            Id.For<LobbyStateMachine>(props.LobbyId),
            nameof(ILobbyStateMachine.GetHost));

        // TODO: Check how this should handle not existing (throw ArgumentException)

        if (props.RequesterId is not null && props.RequesterId != hostId)
            throw new AccessViolationException();

        await ctx.CallActivityAsync(
            nameof(LobbyDeleteActivityFunction),
            new LobbyDeleteActivityFunctionProps(props.LobbyId));
    }
}
