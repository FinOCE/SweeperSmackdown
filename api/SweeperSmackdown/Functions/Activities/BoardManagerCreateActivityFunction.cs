using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class BoardManagerCreateActivityFunctionProps
{
    public string LobbyId { get; }

    public string UserId { get; }

    public GameSettings Settings { get; }

    public BoardManagerCreateActivityFunctionProps(string lobbyId, string userId, GameSettings settings)
    {
        LobbyId = lobbyId;
        UserId = userId;
        Settings = settings;
    }
}

public static class BoardManagerCreateActivityFunction
{
    [FunctionName(nameof(BoardManagerCreateActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableOrchestrationClient orchestrationClient)
    {
        var props = ctx.GetInput<BoardManagerCreateActivityFunctionProps>();

        // Calculate number of remaining boards to send to sub-orchestrator
        var remaining = props.Settings.BoardCount == 0
            ? -1
            : (props.Settings.BoardCount - 1);

        // Start orchestrator
        await orchestrationClient.StartNewAsync(
            nameof(BoardManagerOrchestrationFunction),
            Id.ForInstance(nameof(BoardManagerOrchestrationFunction), props.LobbyId, props.UserId),
            new BoardManagerOrchestrationFunctionProps(props.Settings, remaining));
    }
}
