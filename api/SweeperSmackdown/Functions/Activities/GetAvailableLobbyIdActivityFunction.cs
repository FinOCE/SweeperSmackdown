using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Functions.Entities;
using SweeperSmackdown.Utils;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public static class GetAvailableLobbyIdActivityFunction
{
    [FunctionName(nameof(GetAvailableLobbyIdActivityFunction))]
    public static async Task<string> Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [DurableClient] IDurableEntityClient entityClient)
    {
        while (true)
        {
            var lobbyId = RandomNumberGenerator.GetInt32(100_000).ToString();

            var lobby = await entityClient.ReadEntityStateAsync<LobbyStateMachine>(
                Id.For<LobbyStateMachine>(lobbyId));

            if (!lobby.EntityExists)
                return lobbyId;
        }
    }
}
