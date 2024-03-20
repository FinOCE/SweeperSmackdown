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

namespace SweeperSmackdown.Functions.Http;

public class LobbyPutFunctionPayload
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

public static class LobbyPutFunction
{
    [FunctionName(nameof(LobbyPutFunction))]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "lobbies/{lobbyId}")] LobbyPutFunctionPayload payload,
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

        // Check if lobby exists
        var entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));
        var initiallyExisted = entity.EntityExists;

        if (!initiallyExisted)
        {
            // Create lobby
            await entityClient.SignalEntityAsync<ILobby>(
                Id.For<Lobby>(lobbyId),
                lobby => lobby.Create(
                    lobbyId,
                    Array.Empty<string>(),
                    payload.Lifetime,
                    payload.Mode,
                    payload.Height,
                    payload.Width));

            await orchestrationClient.StartNewAsync(nameof(GameSetupFunction), lobbyId);
        }
        else
        {
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
        }

        // Send events to orchestrator
        var orchestrationTasks = new List<Task>();

        if (payload.Lifetime != null)
            orchestrationTasks.Add(
                orchestrationClient.RaiseEventAsync(
                    Id.ForInstance(nameof(GameSetupFunction), lobbyId),
                    Events.SET_LIFETIME));

        if (payload.Mode != null)
            orchestrationTasks.Add(
                orchestrationClient.RaiseEventAsync(
                    Id.ForInstance(nameof(GameSetupFunction), lobbyId),
                    Events.SET_MODE));

        if (payload.Height != null)
            orchestrationTasks.Add(
                orchestrationClient.RaiseEventAsync(
                    Id.ForInstance(nameof(GameSetupFunction), lobbyId),
                    Events.SET_HEIGHT));

        if (payload.Width != null)
            orchestrationTasks.Add(
                orchestrationClient.RaiseEventAsync(
                    Id.ForInstance(nameof(GameSetupFunction), lobbyId),
                    Events.SET_WIDTH));

        await Task.WhenAll(orchestrationTasks);

        // Return created/updated lobby
        entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));
        return initiallyExisted
            ? new OkObjectResult(entity.EntityState)
            : new CreatedResult($"/lobbies/{lobbyId}", entity.EntityState);
    }
}
