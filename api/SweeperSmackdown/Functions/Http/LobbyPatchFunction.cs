using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Newtonsoft.Json;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public class LobbyPatchFunctionPayload
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

    [JsonProperty("boardCount")]
    public int? BoardCount { get; }

    [JsonProperty("shareBoards")]
    public bool? ShareBoards { get; }
}

public static class LobbyPatchFunction
{
    [FunctionName(nameof(LobbyPatchFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "lobbies/{lobbyId}")] LobbyPatchFunctionPayload payload,
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

        if (payload.Lives != null && payload.Lives < 0)
            errors.Add($"The 'lives' must be greater than or equal to 0 (0 means unlimited)");

        if (payload.TimeLimit != null && payload.TimeLimit < 0)
            errors.Add("The 'timeLimit' must be greater than or equal to 0 (0 means unlimited)");

        if (payload.BoardCount != null && payload.BoardCount < 0)
            errors.Add("The 'boardCount' must be greater than or equal to 0 (0 means unlimited)");

        if (errors.Count > 0)
            return new BadRequestObjectResult(errors);

        // Check if lobby exists and is in configure state
        var entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));

        if (!entity.EntityExists)
            return new NotFoundResult();

        var status = await orchestrationClient.GetStatusAsync(Id.ForInstance(nameof(LobbyOrchestratorFunction), lobbyId));
        
        if (status != null && status.CustomStatus.ToString() != ELobbyOrchestratorFunctionStatus.Configure.ToString())
            return new ConflictResult();

        // Confirm mine count is realistic
        var newHeight = payload.Height ?? entity.EntityState.Settings.Height;
        var newWidth = payload.Width ?? entity.EntityState.Settings.Width;
        var newMines = payload.Mines ?? entity.EntityState.Settings.Mines;

        if (newHeight * newWidth > newMines)
            return new BadRequestObjectResult(new string[]
            {
                "Cannot update because this would result in more mines than there are board squares"
            });

        // Apply changes to entity
        await entityClient.SignalEntityAsync<ILobby>(
            Id.For<Lobby>(lobbyId),
            lobby => lobby.SetSettings(
                entity.EntityState.Settings.Update(
                    payload.Mode,
                    payload.Height,
                    payload.Width,
                    payload.Mines,
                    payload.Lives,
                    payload.TimeLimit,
                    payload.BoardCount,
                    payload.ShareBoards)));

        entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));
        await actions.AddAsync(ActionFactory.UpdateLobby(userId, lobbyId, entity.EntityState));

        // Respond to request
        return new OkObjectResult(entity.EntityState);
    }
}
