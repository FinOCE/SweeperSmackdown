using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System;
using System.Threading.Tasks;
using System.Linq;
using SweeperSmackdown.Functions.Entities;

namespace SweeperSmackdown.Functions.Activities;

public class LobbyDeleteActivityFunctionProps
{
    public string LobbyId { get; set; }

    public LobbyDeleteActivityFunctionProps(string lobbyId)
    {
        LobbyId = lobbyId;
    }
}

public static class LobbyDeleteActivityFunction
{
    [FunctionName(nameof(LobbyDeleteActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient)
    {
        var props = ctx.GetInput<LobbyDeleteActivityFunctionProps>();

        var entity = await entityClient.ReadEntityStateAsync<LobbyStateMachine>(
            Id.For<LobbyStateMachine>(props.LobbyId));

        // Remove all players from lobby
        await Task.WhenAll(
            entity.EntityState.Players
                .Where(p => !p.Active)
                .Select(player => entityClient.SignalEntityAsync<ILobbyStateMachine>(
                    Id.For<LobbyStateMachine>(props.LobbyId),
                    lobby => lobby.RemovePlayer(player.Id))));

        // Delete lobby orchestrators
        await Task.WhenAll(
            entity.EntityState.Players
                .Select(player => Id.ForInstance(
                    nameof(BoardManagerOrchestratorFunction),
                    props.LobbyId,
                    player.Id))
                .Append(Id.ForInstance(nameof(LobbyOrchestratorFunction), props.LobbyId))
                .Select(id =>
                {
                    try
                    {
                        return orchestrationClient.TerminateAsync(id, "Lobby empty");
                    }
                    catch (Exception)
                    {
                        return Task.CompletedTask;
                    };
                }));

        // Delete board entities
        await Task.WhenAll(
            entity.EntityState.Players.Select(player => entityClient
                .SignalEntityAsync<IBoard>(
                    Id.For<Board>(player.Id),
                    board => board.Delete())));

        // Delete state machines
        await entityClient.SignalEntityAsync(
            Id.For<GameSettingsStateMachine>(props.LobbyId),
            nameof(IGameSettingsStateMachine.Delete));

        await entityClient.SignalEntityAsync(
            Id.For<LobbyStateMachine>(props.LobbyId),
            nameof(ILobbyStateMachine.Delete));
    }
}
