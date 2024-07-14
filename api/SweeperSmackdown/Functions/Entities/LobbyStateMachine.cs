using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebJobs;
using SweeperSmackdown.Assets;
using System.Threading.Tasks;
using System;
using System.Runtime.Serialization;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Utils;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Functions.Orchestrators;
using System.Collections.Generic;
using System.Linq;

namespace SweeperSmackdown.Functions.Entities;

public interface ILobbyStateMachine
{
    /// <summary>
    /// Create a new lobby state machine.
    /// </summary>
    /// <param name="hostId">The ID of the lobby's initial host</param>
    public void Create(string hostId);

    /// <summary>
    /// Delete the lobby state machine and all associated states and orchestrators.
    /// </summary>
    public Task Delete();

    /// <summary>
    /// Set a new host for the lobby.
    /// </summary>
    /// <param name="hostId">The ID of the new host</param>
    public Task SetHost(string hostId);

    /// <summary>
    /// Set whether or not the lobby is entirely controlled by the host.
    /// </summary>
    /// <param name="hostManaged">The new state of whether the lobby is host managed</param>
    public Task SetHostManaged(bool hostManaged);

    /// <summary>
    /// Get all players in the lobby, both active and inactive.
    /// </summary>
    /// <returns>A list of all players in the lobby</returns>
    public Task<IEnumerable<Player>> GetPlayers();

    /// <summary>
    /// Add a player to the lobby.
    /// </summary>
    /// <param name="userId">The ID of the player to add</param>
    public Task AddPlayer(string userId);

    /// <summary>
    /// Remove a player from the lobby. This marks them inactive rather than deleting entirely.
    /// </summary>
    /// <param name="userId">The ID of the player to remove</param>
    public Task RemovePlayer(string userId);

    public Task AddScore(string userId);

    public Task AddWin(string userId);
}

[DataContract]
public class LobbyStateMachine : ILobbyStateMachine
{
    private IDurableOrchestrationClient _orchestrationClient { get; }

    private IDurableEntityClient _entityClient { get; }

    private IAsyncCollector<WebPubSubAction> _ws { get; }

    private string LobbyId => Entity.Current.EntityId.EntityKey;

    [DataMember]
    public string HostId { get; private set; } = null!;

    [DataMember]
    public bool HostManaged { get; private set; }

    [DataMember]
    public IEnumerable<Player> Players { get; private set; }

    [FunctionName(nameof(LobbyStateMachine))]
    public static Task Run(
        [EntityTrigger] IDurableEntityContext ctx,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [DurableClient] IDurableEntityClient entityClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws
    ) =>
        ctx.DispatchAsync<LobbyStateMachine>(orchestrationClient, entityClient, ws);

    public LobbyStateMachine(
        IDurableOrchestrationClient orchestrationClient,
        IDurableEntityClient entityClient,
        IAsyncCollector<WebPubSubAction> ws)
    {
        _orchestrationClient = orchestrationClient;
        _entityClient = entityClient;
        _ws = ws;

        Players = Array.Empty<Player>();
    }

    public void Create(string hostId)
    {
        HostId = hostId;
        HostManaged = false;
    }

    public async Task Delete()
    {
        foreach (var player in Players.Where(p => p.Active))
            await RemovePlayer(player.Id);

        Entity.Current.DeleteState();
    }

    public async Task SetHost(string hostId)
    {
        HostId = hostId;

        await _ws.AddAsync(ActionFactory.UpdateLobbyHost(LobbyId, hostId));
    }

    public async Task SetHostManaged(bool hostManaged)
    {
        HostManaged = hostManaged;

        await _ws.AddAsync(ActionFactory.UpdateLobbyHostManaged(LobbyId, hostManaged));
    }

    public Task<IEnumerable<Player>> GetPlayers() =>
        Task.FromResult(Players);

    public async Task AddPlayer(string userId)
    {
        Player player;

        if (Players.Any(p => p.Id == userId))
        {
            player = Players.First(p => p.Id == userId);
            player.Active = true;
            Players = Players.Where(p => p.Id != userId).Append(player);
        }
        else
        {
            player = new Player(userId, LobbyId);
            Players = Players.Append(player);
        }

        await _ws.AddAsync(ActionFactory.JoinLobby(LobbyId, userId));
        await _ws.AddAsync(ActionFactory.AddUserToLobby(userId, LobbyId));
        await _ws.AddAsync(ActionFactory.AddPlayer(LobbyId, player));

        Entity.Current.SignalEntity(
            Id.For<ConnectionReference>(userId),
            nameof(IConnectionReference.SetLobbyId),
            LobbyId);

        var status = await _orchestrationClient.GetStatusAsync(
            Id.ForInstance(nameof(LobbyOrchestratorFunction), LobbyId));

        var customStatus = status.CustomStatus.ToObject<LobbyOrchestratorStatus>()!;

        if (customStatus.Status == ELobbyStatus.Starting || customStatus.Status == ELobbyStatus.Playing)
        {
            var settings = await _entityClient.ReadEntityStateAsync<GameSettingsStateMachine>(
                Id.For<GameSettingsStateMachine>(LobbyId));

            await _orchestrationClient.StartNewAsync(
                nameof(BoardManagerOrchestratorFunction),
                Id.ForInstance(nameof(BoardManagerOrchestratorFunction), LobbyId, userId),
                new BoardManagerOrchestratorFunctionProps(settings.EntityState.Settings));
        }
    }

    public async Task RemovePlayer(string userId)
    {
        Players = Players.Select(p =>
        {
            if (p.Id == userId)
                p.Active = false;

            return p;
        });

        await _ws.AddAsync(ActionFactory.LeaveLobby(LobbyId, userId));
        await _ws.AddAsync(ActionFactory.RemoveUserFromLobby(userId, LobbyId));
        await _ws.AddAsync(ActionFactory.RemovePlayer(LobbyId, userId));

        if (HostId == userId)
        {
            if (Players.All(p => !p.Active))
                await _orchestrationClient.StartNewAsync(
                    nameof(LobbyDeleteOrchestratorFunction),
                    Id.ForInstance(nameof(LobbyDeleteOrchestratorFunction), LobbyId));
            else
                await SetHost(Players.First(p => p.Active).Id);
        }

        Entity.Current.SignalEntity(
            Id.For<ConnectionReference>(userId),
            nameof(IConnectionReference.SetLobbyId),
            "");
    }

    public async Task AddScore(string userId)
    {
        var player = Players.FirstOrDefault(p => p.Id == userId)
            ?? throw new InvalidOperationException();

        player.Score += 1;

        Players = Players.Where(p => p.Id != userId).Append(player);

        await _ws.AddAsync(ActionFactory.UpdatePlayer(LobbyId, player));
    }

    public async Task AddWin(string userId)
    {
        var player = Players.FirstOrDefault(p => p.Id == userId)
            ?? throw new InvalidOperationException();

        player.Wins += 1;

        Players = Players.Where(p => p.Id != userId).Append(player);

        await _ws.AddAsync(ActionFactory.UpdatePlayer(LobbyId, player));
    }
}
