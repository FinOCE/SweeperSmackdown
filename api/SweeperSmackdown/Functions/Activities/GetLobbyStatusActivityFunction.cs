using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class GetLobbyStatusActivityFunctionProps
{
    public string LobbyId { get; set; }

    public GetLobbyStatusActivityFunctionProps(string lobbyId)
    {
        LobbyId = lobbyId;
    }
}

public static class GetLobbyStatusActivityFunction
{
    [FunctionName(nameof(GetLobbyStatusActivityFunction))]
    public static async Task<PreciseLobbyStatus?> Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient)
    {
        var props = ctx.GetInput<GetLobbyStatusActivityFunctionProps>();

        var settings = await entityClient.ReadEntityStateAsync<GameSettingsStateMachine>(
            Id.For<GameSettingsStateMachine>(props.LobbyId));

        if (!settings.EntityExists)
            return null;

        var status = await orchestrationClient.GetStatusAsync(
            Id.ForInstance(nameof(LobbyOrchestratorFunction), props.LobbyId));

        var customStatus = status.CustomStatus.ToObject<LobbyOrchestratorStatus>();

        if (customStatus is null)
            return null;

        return new PreciseLobbyStatus(
            customStatus,
            customStatus.Status == ELobbyStatus.Configuring
                ? settings.EntityState.State
                : null);
    }
}
