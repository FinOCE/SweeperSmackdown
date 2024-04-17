﻿using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Extensions;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Activities;

public class BoardCreatedActivityFunctionProps
{
    public string LobbyId { get; }

    public string UserId { get; }

    public byte[] GameState { get; }

    public BoardCreatedActivityFunctionProps(string lobbyId, string userId, byte[] gameState)
    {
        LobbyId = lobbyId;
        UserId = userId;
        GameState = gameState;
    }
}

public static class BoardCreatedActivityFunction
{
    [FunctionName(nameof(BoardCreatedActivityFunction))]
    public static async Task Run(
        [ActivityTrigger] IDurableActivityContext ctx,
        [CosmosDB(Connection = "CosmosDbConnectionString")] CosmosClient cosmosClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws)
    {
        var props = ctx.GetInput<BoardCreatedActivityFunctionProps>();
        
        // Notify user of created game state
        await ws.AddAsync(ActionFactory.CreateBoard(props.UserId, props.LobbyId, props.GameState, false));

        // Update board entity map
        var container = cosmosClient.GetBoardContainer();

        BoardEntityMap boardEntityMap = await container.ReadItemAsync<BoardEntityMap>(
            props.LobbyId,
            new(props.LobbyId));

        if (!boardEntityMap.BoardIds.Contains(props.UserId))
            await container.PatchItemAsync<BoardEntityMap>(props.LobbyId, new(props.LobbyId), new[]
            {
                PatchOperation.Add("/boardIds/-", props.UserId)
            });
    }
}
