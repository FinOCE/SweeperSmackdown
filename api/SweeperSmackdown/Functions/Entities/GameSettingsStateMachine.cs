using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using Microsoft.Azure.WebJobs;
using System;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using SweeperSmackdown.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace SweeperSmackdown.Functions.Entities;

/// <summary>
/// State machine to configure game settings for a lobby.
/// </summary>
public interface IGameSettingsStateMachine
{
    /// <summary>
    /// Create a new lobby's game settings state machine.
    /// </summary>
    /// <param name="initialSettings">The initial game settings to use</param>
    void Create(GameSettings initialSettings);

    /// <summary>
    /// Delete the state machine.
    /// </summary>
    void Delete();

    /// <summary>
    /// Gets the current game settings.
    /// </summary>
    /// <returns>The current game settings</returns>
    Task<GameSettings> GetSettings();

    /// <summary>
    /// Set the game settings for the lobby.
    /// </summary>
    /// <param name="settings">The new settings to use</param>
    Task UpdateSettings(GameSettingsUpdateRequest updates);

    /// <summary>
    /// Gets the current state of the state machine.
    /// </summary>
    /// <returns>The current state of the state machine</returns>
    Task<EGameSettingsStateMachineState> GetState();

    /// <summary>
    /// Lock the game settings to what they currently are if currently unlocked.
    /// </summary>
    Task Lock();

    /// <summary>
    /// Unlocks the game settings to be modified if currently locked.
    /// </summary>
    Task Unlock();

    /// <summary>
    /// Confirms the current game settings for use and notifies orchestrator if currently locked.
    /// </summary>
    Task Confirm();

    /// <summary>
    /// Opens the game settings to be modified again for a new round if currently confirmed.
    /// </summary>
    Task Open();
}

[DataContract]
public class GameSettingsStateMachine : IGameSettingsStateMachine
{
    public static IEnumerable<EGameSettingsStateMachineState> ValidStatesToUpdateSettings { get; } = new[]
    {
        EGameSettingsStateMachineState.Unlocked
    };

    public static IEnumerable<EGameSettingsStateMachineState> ValidStatesToLock { get; } = new[]
    {
        EGameSettingsStateMachineState.Unlocked
    };

    public static IEnumerable<EGameSettingsStateMachineState> ValidStatesToUnlock { get; } = new[]
    {
        EGameSettingsStateMachineState.Locked
    };

    public static IEnumerable<EGameSettingsStateMachineState> ValidStatesToConfirm { get; } = new[]
    {
        EGameSettingsStateMachineState.Locked
    };

    public static IEnumerable<EGameSettingsStateMachineState> ValidStatesToOpen { get; } = new[]
    {
        EGameSettingsStateMachineState.Confirmed
    };

    private IDurableOrchestrationClient _orchestrationClient { get; }

    private IAsyncCollector<WebPubSubAction> _ws { get; }

    [DataMember]
    public GameSettings Settings { get; private set; } = null!;

    [DataMember]
    public EGameSettingsStateMachineState State { get; private set; }

    [FunctionName(nameof(GameSettingsStateMachine))]
    public static Task Run(
        [EntityTrigger] IDurableEntityContext ctx,
        [DurableClient] IDurableOrchestrationClient orchestrationClient,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws
    ) =>
        ctx.DispatchAsync<GameSettingsStateMachine>(orchestrationClient, ws);

    public GameSettingsStateMachine(
        IDurableOrchestrationClient orchestrationClient,
        IAsyncCollector<WebPubSubAction> ws)
    {
        _orchestrationClient = orchestrationClient;
        _ws = ws;
    }

    public void Create(GameSettings initialSettings)
    {
        if (Entity.Current.HasState)
            throw new InvalidOperationException();

        Settings = initialSettings;
        State = EGameSettingsStateMachineState.Unlocked;
    }

    public void Delete() =>
        Entity.Current.DeleteState();

    public Task<GameSettings> GetSettings() =>
        Task.FromResult(Settings);

    public async Task UpdateSettings(GameSettingsUpdateRequest updates)
    {
        if (!ValidStatesToUpdateSettings.Contains(State))
            throw new InvalidOperationException();

        Settings = updates.ApplyToModel(Settings);
        await _ws.AddAsync(ActionFactory.UpdateLobbySettings(Entity.Current.EntityId.EntityKey, Settings));
    }

    public Task<EGameSettingsStateMachineState> GetState() =>
        Task.FromResult(State);

    public async Task Lock()
    {
        if (!ValidStatesToLock.Contains(State))
            throw new InvalidOperationException();

        State = EGameSettingsStateMachineState.Locked;
        await _ws.AddAsync(ActionFactory.UpdateLobbyState(Entity.Current.EntityId.EntityKey, State));
    }

    public async Task Unlock()
    {
        if (!ValidStatesToUnlock.Contains(State))
            throw new InvalidOperationException();

        State = EGameSettingsStateMachineState.Unlocked;
        await _ws.AddAsync(ActionFactory.UpdateLobbyState(Entity.Current.EntityId.EntityKey, State));
    }

    public async Task Confirm()
    {
        if (!ValidStatesToConfirm.Contains(State))
            throw new InvalidOperationException();

        State = EGameSettingsStateMachineState.Confirmed;
        await _ws.AddAsync(ActionFactory.UpdateLobbyState(Entity.Current.EntityId.EntityKey, State));

        await _orchestrationClient.RaiseEventAsync(
            Id.ForInstance(nameof(GameConfigureFunction), Entity.Current.EntityId.EntityKey),
            DurableEvents.LOBBY_CONFIRMED,
            Settings);
    }

    public async Task Open()
    {
        if (!ValidStatesToOpen.Contains(State))
            throw new InvalidOperationException();

        State = EGameSettingsStateMachineState.Unlocked;
        await _ws.AddAsync(ActionFactory.UpdateLobbyState(Entity.Current.EntityId.EntityKey, State));
    }
}

public enum EGameSettingsStateMachineState
{
    Unlocked,
    Locked,
    Confirmed
}
