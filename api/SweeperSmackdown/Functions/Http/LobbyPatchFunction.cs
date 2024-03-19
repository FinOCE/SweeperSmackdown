using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using SweeperSmackdown.Entities;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
using System.Collections.Generic;
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
