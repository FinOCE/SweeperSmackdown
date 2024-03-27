using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Utils;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SweeperSmackdown.Functions.Entities;

public interface IBoard
{
    void Create(byte[] gameState);

    void Delete();

    Task<byte[]> GetInitialState();

    Task<byte[]> GetGameState();

    void MakeMove(int index, byte state);
}

[DataContract]
public class Board : IBoard
{
    [DataMember]
    public byte[] InitialState { get; private set; } = null!;
    [DataMember]
    public byte[] GameState { get; private set; } = null!;

    public void Create(byte[] gameState)
    {
        InitialState = gameState;
        GameState = gameState;
    }

    public void Delete() =>
        Entity.Current.DeleteState();

    public Task<byte[]> GetInitialState() =>
        Task.FromResult(InitialState);

    public Task<byte[]> GetGameState() =>
        Task.FromResult(GameState);

    public void MakeMove(int index, byte state)
    {
        if (!State.IsRevealedEquivalent(InitialState[index], state))
            throw new ArgumentException("The new game state must not have changed the immutable board data");
        
        GameState[index] = state;
    }

    [FunctionName(nameof(Board))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx) =>
        ctx.DispatchAsync<Board>();
}
