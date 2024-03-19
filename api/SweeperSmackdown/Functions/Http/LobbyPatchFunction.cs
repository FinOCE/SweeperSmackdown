using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using SweeperSmackdown.Entities;
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
        [DurableClient] IDurableEntityClient entityClient,
        string lobbyId)
    {
        var entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));

        if (!entity.EntityExists)
            return new NotFoundResult();

        if (entity.EntityState.Status != ELobbyStatus.Setup)
            return new ConflictResult();

        var tasks = new List<Task>();

        if (payload.Lifetime != null)
            tasks.Add(
                entityClient.SignalEntityAsync<ILobby>(
                    Id.For<Lobby>(lobbyId),
                    lobby => lobby.SetLifetime(payload.Lifetime.Value)));

        if (payload.Mode != null)
            tasks.Add(
                entityClient.SignalEntityAsync<ILobby>(
                    Id.For<Lobby>(lobbyId),
                    lobby => lobby.SetMode(payload.Mode.Value)));

        if (payload.Height != null)
            tasks.Add(
                entityClient.SignalEntityAsync<ILobby>(
                    Id.For<Lobby>(lobbyId),
                    lobby => lobby.SetHeight(payload.Height.Value)));

        if (payload.Width != null)
            tasks.Add(
                entityClient.SignalEntityAsync<ILobby>(
                    Id.For<Lobby>(lobbyId),
                    lobby => lobby.SetWidth(payload.Width.Value)));        
        
        await Task.WhenAll(tasks);

        entity = await entityClient.ReadEntityStateAsync<Lobby>(Id.For<Lobby>(lobbyId));
        return new OkObjectResult(entity.EntityState);
    }
}
