using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Orchestrators;

public class GameActiveFunctionProps
{
    public GameSettings Settings { get; }

    public GameActiveFunctionProps(GameSettings settings)
    {
        Settings = settings;
    }
}

public static class GameActiveFunction
{
    [FunctionName(nameof(GameActiveFunction))]
    public static async Task<string?> Run(
        [OrchestrationTrigger] IDurableOrchestrationContext ctx)
    {
        var lobbyId = Id.FromInstance(ctx.InstanceId);
        var props = ctx.GetInput<GameActiveFunctionProps>();

        // TODO: Notify users the game has started

        // Wait until a user completes their board or timeout
        var timerTask = ctx.CallSubOrchestratorAsync(
            nameof(TimerOrchestratorFunction),
            Id.ForInstance(nameof(TimerOrchestratorFunction), lobbyId),
            new TimerOrchestratorFunctionProps(props.Settings.TimeLimit, true));

        var eventTask = ctx.WaitForExternalEvent<string>(DurableEvents.GAME_COMPLETED);
        
        var winner = await Task.WhenAny(timerTask, eventTask);

        // Determine the winner
        return (winner == eventTask)
            ? eventTask.Result
            : null;
    }
}
