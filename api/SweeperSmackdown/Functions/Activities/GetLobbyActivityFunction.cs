using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.DTOs;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class GetLobbyActivityFunctionProps
{
    public string LobbyId { get; set; }

    public GetLobbyActivityFunctionProps(string lobbyId)
    {
        LobbyId = lobbyId;
    }
}

public static class GetLobbyActivityFunction
{
    [FunctionName(nameof(GetLobbyActivityFunction))]
    public static async Task<LobbyResponse?> Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient)
    {
        var props = ctx.GetInput<GetLobbyActivityFunctionProps>();

        // Get current entity state
        var lobby = await entityClient.ReadEntityStateAsync<LobbyStateMachine>(
            Id.For<LobbyStateMachine>(props.LobbyId));

        var settings = await entityClient.ReadEntityStateAsync<GameSettingsStateMachine>(
            Id.For<GameSettingsStateMachine>(props.LobbyId));

        // Get status
        var orchestratorStatus = await orchestrationClient.GetStatusAsync(
            Id.ForInstance(nameof(LobbyOrchestratorFunction), props.LobbyId));

        var customStatus = orchestratorStatus.CustomStatus.ToObject<LobbyOrchestratorStatus>();

        var status = customStatus is null
            ? null
            : new PreciseLobbyStatus(
                customStatus,
                customStatus.Status == ELobbyStatus.Configuring
                    ? settings.EntityState.State
                    : null);

        // TODO: All fetching above can probably be optionally passed in as props to save time

        if (!lobby.EntityExists || !settings.EntityExists || status is null)
            return null;

        return LobbyResponse.FromModel(
            props.LobbyId,
            status,
            lobby.EntityState,
            settings.EntityState);
    }
}
