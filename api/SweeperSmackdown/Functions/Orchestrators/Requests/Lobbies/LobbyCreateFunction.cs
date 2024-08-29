using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators.Requests.Lobbies;

public class LobbyCreateFunctionProps
{
    public string? LobbyId { get; set; }

    public string HostId { get; set; }

    public LobbyCreateFunctionProps(string? lobbyId, string hostId)
    {
        LobbyId = lobbyId;
        HostId = hostId;
    }
}

public static class LobbyCreateFunction
{
    [FunctionName(nameof(LobbyCreateFunction))]
    public static async Task<LobbyResponse> Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var props = ctx.GetInput<LobbyCreateFunctionProps>();

        // Ensure lobby ID is available, or generate available one
        string lobbyId;
        if (props.LobbyId is not null)
        {
            var exists = await ctx.CallActivityAsync<bool>(
                nameof(CheckLobbyIdAvailableActivityFunction),
                new CheckLobbyIdAvailableActivityFunctionProps(props.LobbyId));

            if (exists)
                throw new ArgumentException($"Lobby ID {props.LobbyId} is already in use");

            lobbyId = props.LobbyId;
        } else
        {
            lobbyId = await ctx.CallActivityAsync<string>(
                nameof(GetAvailableLobbyIdActivityFunction),
                null);

            // TODO: Create single entity to track currently used IDs rather than randomly checking. This will also fix
            //       possible race conditions where two lobbies made with same ID at same time.
        }

        // Create state machines
        await ctx.CallEntityAsync(
            Id.For<LobbyStateMachine>(lobbyId),
            nameof(ILobbyStateMachine.Create),
            props.HostId);

        await ctx.CallEntityAsync(
            Id.For<GameSettingsStateMachine>(lobbyId),
            nameof(IGameSettingsStateMachine.Create),
            new GameSettings());

        // Start lobby
        ctx.StartNewOrchestration(
            nameof(LobbyOrchestratorFunction),
            null,
            Id.ForInstance(nameof(LobbyOrchestratorFunction), lobbyId));

        await ctx.CallActivityAsync(
            nameof(NotifyActivityFunction),
            new NotifyActivityFunctionProps(ActionFactory.CreatedLobby(lobbyId, props.HostId)));

        // TODO: Probably delete all uses of NotifyActivityFunction and use polling instead

        return await ctx.CallSubOrchestratorAsync<LobbyResponse>(
            nameof(LobbyFetchFunction),
            new LobbyFetchFunctionProps(lobbyId));
    }
}
