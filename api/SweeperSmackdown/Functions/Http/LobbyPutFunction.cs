using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Structures;

namespace SweeperSmackdown.Functions.Http;

public class LobbyPutFunctionPayload
{
    [JsonProperty("mode")]
    public int? Mode { get; }

    [JsonProperty("height")]
    public int? Height { get; }

    [JsonProperty("width")]
    public int? Width { get; }

    [JsonProperty("mines")]
    public int? Mines { get; }

    [JsonProperty("lives")]
    public int? Lives { get; }

    [JsonProperty("timeLimit")]
    public int? TimeLimit { get; }
}

public static class LobbyPutFunction
{
    [FunctionName(nameof(LobbyPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}")] LobbyPutFunctionPayload payload,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> actions)
    {
        // TODO: Get userId for person that made request
        var userId = "userId";

        // Handle validation failures
        var errors = new List<string>();

        if (payload.Mode != null && !GameStateFactory.VALID_MODES.Contains(payload.Mode.Value))
            errors.Add($"The 'mode' is not a valid option ({string.Join(", ", GameStateFactory.VALID_MODES)})");

        if (payload.Height != null && payload.Height >= Constants.MAX_GAME_HEIGHT || payload.Height <= Constants.MIN_GAME_HEIGHT)
            errors.Add($"The 'height' must be between {Constants.MIN_GAME_HEIGHT} and {Constants.MAX_GAME_HEIGHT}");

        if (payload.Width != null && payload.Width >= Constants.MAX_GAME_WIDTH || payload.Width <= Constants.MIN_GAME_WIDTH)
            errors.Add($"The 'width' must be between {Constants.MIN_GAME_WIDTH} and {Constants.MAX_GAME_WIDTH}");

        if (payload.Mines != null && payload.Mines <= 0)
            errors.Add($"The 'mines' must be greater than 0");

        if (payload.Lives != null && payload.Lives < -1)
            errors.Add($"The 'lives' must be greater than or equal to 0 (0 means unlimited)");

        if (payload.TimeLimit != null && payload.TimeLimit <= 0)
            errors.Add("The 'timeLimit' must be greater than or equal to 0 (0 means unlimited)");

        if (errors.Count > 0)
            return new BadRequestObjectResult(errors);

        // Check if lobby exists
        var entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));
        var initiallyExisted = entity.EntityExists;

        // Confirm mine count is realistic
        var defaultSettings = new GameSettings();
        var newHeight = payload.Height ?? entity.EntityState?.Settings?.Height ?? defaultSettings.Height;
        var newWidth = payload.Width ?? entity.EntityState?.Settings?.Width ?? defaultSettings.Width;
        var newMines = payload.Mines ?? entity.EntityState?.Settings?.Mines ?? defaultSettings.Height;

        if (newHeight * newWidth > newMines)
            return new BadRequestObjectResult(new string[]
            {
                "Cannot update because this would result in more mines than there are board squares"
            });

        if (!initiallyExisted)
        {
            // Create lobby
            await entityClient.SignalEntityAsync<ILobby>(
                Id.For<Lobby>(lobbyId),
                lobby => lobby.Create((
                    lobbyId,
                    Array.Empty<string>(),
                    new GameSettings().Update(
                        payload.Mode,
                        payload.Height,
                        payload.Width,
                        payload.Mines,
                        payload.Lives,
                        payload.TimeLimit))));

            await orchestrationClient.StartNewAsync(nameof(GameConfigureFunction), lobbyId);
        }
        else
        {
            // Apply changes to entity
            await entityClient.SignalEntityAsync<ILobby>(
                Id.For<Lobby>(lobbyId),
                lobby => lobby.SetSettings(
                    entity.EntityState!.Settings.Update(
                        payload.Mode,
                        payload.Height,
                        payload.Width,
                        payload.Mines,
                        payload.Lives,
                        payload.TimeLimit)));
        }

        entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));
        await actions.AddAsync(ActionFactory.UpdateLobby(userId, lobbyId, entity.EntityState));

        // Return created/updated lobby        
        return initiallyExisted
            ? new OkObjectResult(entity.EntityState)
            : new CreatedResult($"/lobbies/{lobbyId}", entity.EntityState);
    }
}
