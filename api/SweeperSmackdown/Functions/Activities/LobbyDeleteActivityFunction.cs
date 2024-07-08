using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Models;
using SweeperSmackdown.Utils;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using SweeperSmackdown.Extensions;
using System.Linq;
using SweeperSmackdown.Functions.Entities;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;

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
        [DurableClient] IDurableEntityClient entityClient,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var props = ctx.GetInput<LobbyDeleteActivityFunctionProps>();

        var players = await cosmosClient.GetAllPlayersInLobbyAsync(props.LobbyId);

        // Notify AFK players to kick
        await ws.AddAsync(ActionFactory.DeleteLobby(props.LobbyId));

        // Delete lobby db entry
        await cosmosClient
            .GetLobbyContainer()
            .DeleteItemAsync<Lobby>(props.LobbyId, new(props.LobbyId));

        // Delete lobby orchestrators
        List<string> orchestrationIds = new()
        {
            Id.ForInstance(nameof(OldLobbyOrchestratorFunction), props.LobbyId),
            Id.ForInstance(nameof(GameActiveFunction), props.LobbyId),
            Id.ForInstance(nameof(GameCelebrationFunction), props.LobbyId),
            Id.ForInstance(nameof(GameCleanupFunction), props.LobbyId),
            Id.ForInstance(nameof(GameConfigureFunction), props.LobbyId)
        };

        orchestrationIds.AddRange(players.Select(player =>
            Id.ForInstance(nameof(BoardManagerOrchestratorFunction), props.LobbyId, player.Id)));

        foreach (var id in orchestrationIds)
            try
            {
                await orchestrationClient.TerminateAsync(id, "Lobby empty");
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to terminate a board manager orchestration (DOES THIS EVER OCCUR?)");
            }

        // Delete board entities
        await Task.WhenAll(
            players.Select(player => entityClient
                .SignalEntityAsync<IBoard>(
                    Id.For<Board>(player.Id),
                    board => board.Delete())));

        // Delete player db entries
        await Task.WhenAll(
            players.Select(p => cosmosClient
                .GetPlayerContainer()
                .DeleteItemAsync<Player>(p.Id, new(p.LobbyId))));
    }
}
