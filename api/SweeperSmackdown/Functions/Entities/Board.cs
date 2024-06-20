using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.WebPubSub;
using SweeperSmackdown.Assets;
using SweeperSmackdown.DTOs.Websocket;
using SweeperSmackdown.Factories;
using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Entities;

public interface IBoard
{
    void Create((string LobbyId, byte[] GameState, int Lives) args);

    void Delete();

    Task<byte[]> GetInitialState();

    Task<byte[]> GetGameState();
    
    Task MakeMove((DateTime TriggerTime, OnMoveData Data) args);

    void Reset();
}

[DataContract]
public class Board : IBoard
{
    private IAsyncCollector<WebPubSubAction> _ws { get; set; } = null!;

    [DataMember]
    public string LobbyId { get; private set; } = null!;

    [DataMember]
    public string UserId { get; private set; } = null!;

    [DataMember]
    public byte[] InitialState { get; private set; } = null!;

    [DataMember]
    public byte[] GameState { get; private set; } = null!;

    [DataMember]
    public int InitialLives { get; private set; }

    [DataMember]
    public int Lives { get; private set; }

    [DataMember]
    public DateTime? DisabledUntil { get; set; }

    public bool IsDisabled
    {
        get { return Lives == 0 || DisabledUntil is not null && DisabledUntil.Value < DateTime.UtcNow; }
    }

    public Board(IAsyncCollector<WebPubSubAction> ws)
    {
        _ws = ws;
    }

    public void Create((string LobbyId, byte[] GameState, int Lives) args)
    {
        LobbyId = args.LobbyId;
        UserId = Entity.Current.EntityKey;
        InitialState = args.GameState;
        GameState = args.GameState;
        InitialLives = args.Lives;
        Lives = args.Lives;
        DisabledUntil = null;
    }

    public void Delete() =>
        Entity.Current.DeleteState();

    public Task<byte[]> GetInitialState() =>
        Task.FromResult(InitialState);

    public Task<byte[]> GetGameState() =>
        Task.FromResult(GameState);

    public async Task MakeMove((DateTime TriggerTime, OnMoveData Data) args)
    {
        var data = args.Data;
        var triggerTime = args.TriggerTime; // Reduce timeout delay from trigger -> entity

        try
        {
            var playerStateUpdated = false;

            if (IsDisabled)
                throw new InvalidOperationException("Moves are currently disabled");

            if (data.FlagAdd != null)
            {
                // Set flag on state
                GameState = GameState
                    .Select((state, i) => data.FlagAdd == i ? State.Flag(state) : state)
                    .ToArray();
            }
            else if (data.FlagRemove != null)
            {
                // Remove flag on state
                GameState = GameState
                    .Select((state, i) => data.FlagRemove == i ? State.RemoveFlag(state) : state)
                    .ToArray();
            }
            else if (data.Reveals != null)
            {
                // Reveal tiles in state
                GameState = GameState
                    .Select((state, i) => data.Reveals.Contains(i) ? State.Reveal(state) : state)
                    .ToArray();

                foreach (var reveal in data.Reveals)
                    if (State.IsBomb(GameState[reveal]) && Lives != -1 && Lives != 0)
                    {
                        Lives--;
                        DisabledUntil = triggerTime.AddSeconds(3);
                        playerStateUpdated = true;
                    }
            }

            await _ws.AddAsync(ActionFactory.MakeMove(UserId, LobbyId, data));

            if (playerStateUpdated)
                await _ws.AddAsync(ActionFactory.UpdatePlayerState(UserId, LobbyId, PlayerState.FromEntity(this)));
        }
        catch (InvalidOperationException)
        {
            await _ws.AddAsync(ActionFactory.RejectMove(UserId, data));
        }
    }

    public void Reset()
    {
        GameState = InitialState;
        Lives = InitialLives;
    }

    [FunctionName(nameof(Board))]
    public static Task Run(
        [EntityTrigger] IDurableEntityContext ctx,
        [WebPubSub(Hub = PubSubConstants.HUB_NAME)] IAsyncCollector<WebPubSubAction> ws
    ) =>
        ctx.DispatchAsync<Board>(ws);
}
