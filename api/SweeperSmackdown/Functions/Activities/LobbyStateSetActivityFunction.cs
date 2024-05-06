using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Models;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class LobbyStateSetActivityFunctionProps
{
    public string LobbyId { get; set; }
    
    public ELobbyState State { get; set; }

    public LobbyStateSetActivityFunctionProps(string lobbyId, ELobbyState state)
    {
        LobbyId = lobbyId;
        State = state;
    }
}

public static class LobbyStateSetActivityFunction
{
    [FunctionName(nameof(LobbyStateSetActivityFunction))]
    public static async Task<Lobby?> Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient)
    {
        var props = ctx.GetInput<LobbyStateSetActivityFunctionProps>();
        var container = cosmosClient.GetLobbyContainer();

        try
        {
            Lobby lobby = await container.ReadItemAsync<Lobby>(props.LobbyId, new(props.LobbyId));

            lobby.State = props.State;
            await container.UpsertItemAsync(lobby);
            
            return lobby;
        }
        catch (CosmosException)
        {
            return null;
        }
    }
}
