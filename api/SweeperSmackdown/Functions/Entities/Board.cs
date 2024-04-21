using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.DTOs.Websocket;
using SweeperSmackdown.Utils;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Entities;

public interface IBoard
{
    void Create((byte[] GameState, int Lives) args);

    void Delete();

    Task<byte[]> GetInitialState();

    Task<byte[]> GetGameState();
    
    void MakeMove(OnMoveData data);

    void Reset();
}

[DataContract]
public class Board : IBoard
{
    [DataMember]
    public byte[] InitialState { get; private set; } = null!;

    [DataMember]
    public byte[] GameState { get; private set; } = null!;

    [DataMember]
    public int InitialLives { get; private set; }

    [DataMember]
    public int Lives { get; private set; }

    public void Create((byte[] GameState, int Lives) args)
    {
        InitialState = args.GameState;
        GameState = args.GameState;
        InitialLives = args.Lives;
        Lives = args.Lives;
    }

    public void Delete() =>
        Entity.Current.DeleteState();

    public Task<byte[]> GetInitialState() =>
        Task.FromResult(InitialState);

    public Task<byte[]> GetGameState() =>
        Task.FromResult(GameState);

    public void MakeMove(OnMoveData data)
    {
        if (Lives == 0)
            throw new InvalidOperationException("No lives remaining");

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
                    Lives--;
        }
    }

    public void Reset()
    {
        GameState = InitialState;
        Lives = InitialLives;
    }

    [FunctionName(nameof(Board))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx) =>
        ctx.DispatchAsync<Board>();
}
