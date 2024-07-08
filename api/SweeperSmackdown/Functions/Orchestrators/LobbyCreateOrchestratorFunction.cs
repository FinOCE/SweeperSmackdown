using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Activities;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public class LobbyCreateOrchestratorFunctionProps
{
    public string HostId { get; set; }

    public LobbyCreateOrchestratorFunctionProps(string hostId)
    {
        HostId = hostId;
    }
}

public static class LobbyCreateOrchestratorFunction
{
    [FunctionName(nameof(LobbyCreateOrchestratorFunction))]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var props = ctx.GetInput<LobbyCreateOrchestratorFunctionProps>();

        await ctx.CallEntityAsync(
            Id.For<LobbyStateMachine>(lobbyId),
            nameof(ILobbyStateMachine.Create),
            props.HostId);

        await ctx.CallEntityAsync(
            Id.For<GameSettingsStateMachine>(lobbyId),
            nameof(IGameSettingsStateMachine.Create),
            new GameSettings());

        ctx.StartNewOrchestration(
            nameof(LobbyOrchestratorFunction),
            null,
            Id.ForInstance(nameof(LobbyOrchestratorFunction), lobbyId));

        await ctx.CallActivityAsync(
            nameof(NotifyActivityFunction),
            new NotifyActivityFunctionProps(ActionFactory.CreatedLobby(lobbyId, props.HostId)));
    }
}
