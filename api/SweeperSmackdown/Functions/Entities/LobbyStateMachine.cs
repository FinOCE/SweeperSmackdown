﻿using Microsoft.Azure.WebJobs.Extensions.DurableTask;
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
using SweeperSmackdown.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SweeperSmackdown.Functions.Entities;

public interface ILobbyStateMachine
{
    /// <summary>
    /// Create a new lobby state machine. Also creates the game settings state machine.
    /// </summary>
    /// <param name="hostId">The ID of the lobby's initial host</param>
    public Task Create(string hostId);

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
}

[DataContract]
public class LobbyStateMachine : ILobbyStateMachine
{
    private IDurableOrchestrationClient _orchestrationClient { get; }

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
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws
    ) =>
        ctx.DispatchAsync<LobbyStateMachine>(orchestrationClient, ws);

    public LobbyStateMachine(
        IDurableOrchestrationClient orchestrationClient,
        IAsyncCollector<WebPubSubAction> ws)
    {
        _orchestrationClient = orchestrationClient;
        _ws = ws;

        Players = Array.Empty<Player>();
    }

    public async Task Create(string hostId)
    {
        if (Entity.Current.HasState)
            throw new InvalidOperationException();

        HostId = hostId;
        HostManaged = false;

        Entity.Current.SignalEntity(
            Id.For<GameSettingsStateMachine>(LobbyId),
            nameof(IGameSettingsStateMachine.Create),
            new GameSettings());

        await AddPlayer(hostId);
    }

    public async Task Delete()
    {
        foreach (var player in Players.Where(p => p.Active))
            await RemovePlayer(player.Id);

        await _orchestrationClient.StartNewAsync(
            nameof(LobbyDeleteOrchestratorFunction),
            Id.ForInstance(nameof(LobbyDeleteOrchestratorFunction), LobbyId));

        Entity.Current.SignalEntity(
            Id.For<GameSettingsStateMachine>(LobbyId),
            nameof(IGameSettingsStateMachine.Delete));

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
                await Delete();
            else
                await SetHost(Players.First(p => p.Active).Id);
        }
    }
}