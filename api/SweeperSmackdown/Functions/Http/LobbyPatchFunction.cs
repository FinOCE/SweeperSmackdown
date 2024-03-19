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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Http;

public class LobbyPatchFunctionPayload
{
    [JsonProperty("lifetime")]
    public int? Lifetime { get; }

    [JsonProperty("mode")]
    public int? Mode { get; }

    [JsonProperty("height")]
    public int? Height { get; }

    [JsonProperty("width")]
    public int? Width { get; }
}

public static class LobbyPatchFunction
{
    [FunctionName(nameof(LobbyPatchFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "lobbies/{lobbyId}")] LobbyPatchFunctionPayload payload,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        // Handle validation failures
        var errors = new List<string>();

        if (payload.Lifetime != null && payload.Lifetime <= 0)
            errors.Add("The 'lifetime' must be greater than 0");

        if (payload.Mode != null && !GameStateFactory.VALID_MODES.Contains(payload.Mode.Value))
            errors.Add($"The 'mode' is not a valid option ({string.Join(", ", GameStateFactory.VALID_MODES)})");

        if (payload.Height != null && payload.Height >= Constants.MAX_GAME_HEIGHT || payload.Height <= Constants.MIN_GAME_HEIGHT)
            errors.Add($"The 'height' must be between {Constants.MIN_GAME_HEIGHT} and {Constants.MAX_GAME_HEIGHT}");

        if (payload.Width != null && payload.Width >= Constants.MAX_GAME_WIDTH || payload.Width <= Constants.MIN_GAME_WIDTH)
            errors.Add($"The 'width' must be between {Constants.MIN_GAME_WIDTH} and {Constants.MAX_GAME_WIDTH}");

        if (errors.Count > 0)
            return new BadRequestObjectResult(errors);

        // Check if lobby exists and is in setup
        var entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));

        if (!entity.EntityExists)
            return new NotFoundResult();

        if (entity.EntityState.Status != ELobbyStatus.Setup)
            return new ConflictResult();

        // Apply changes to entity
        var entityTasks = new List<Task>();

        if (payload.Lifetime != null)
            entityTasks.Add(
                entityClient.SignalEntityAsync<ILobby>(
                    Id.For<Lobby>(lobbyId),
                    lobby => lobby.SetLifetime(payload.Lifetime.Value)));

        if (payload.Mode != null)
            entityTasks.Add(
                entityClient.SignalEntityAsync<ILobby>(
                    Id.For<Lobby>(lobbyId),
                    lobby => lobby.SetMode(payload.Mode.Value)));

        if (payload.Height != null)
            entityTasks.Add(
                entityClient.SignalEntityAsync<ILobby>(
                    Id.For<Lobby>(lobbyId),
                    lobby => lobby.SetHeight(payload.Height.Value)));

        if (payload.Width != null)
            entityTasks.Add(
                entityClient.SignalEntityAsync<ILobby>(
                    Id.For<Lobby>(lobbyId),
                    lobby => lobby.SetWidth(payload.Width.Value)));        
        
        await Task.WhenAll(entityTasks);

        // Send events to orchestrator
        var orchestrationTasks = new List<Task>();
        
        if (payload.Lifetime != null)
            orchestrationTasks.Add(
                orchestrationClient.RaiseEventAsync(
                    Id.ForInstance(nameof(GameSetupFunction), lobbyId),
                    "SetLifetime"));
        
        if (payload.Mode != null)
            orchestrationTasks.Add(
                orchestrationClient.RaiseEventAsync(
                    Id.ForInstance(nameof(GameSetupFunction), lobbyId),
                    "SetMode"));

        if (payload.Height != null)
            orchestrationTasks.Add(
                orchestrationClient.RaiseEventAsync(
                    Id.ForInstance(nameof(GameSetupFunction), lobbyId),
                    "SetHeight"));

        if (payload.Width != null)
            orchestrationTasks.Add(
                orchestrationClient.RaiseEventAsync(
                    Id.ForInstance(nameof(GameSetupFunction), lobbyId),
                    "SetWidth"));

        await Task.WhenAll(orchestrationTasks);

        // Respond to request
        entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));
        return new OkObjectResult(entity.EntityState);
    }
}
