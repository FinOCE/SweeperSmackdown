using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Utils;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SweeperSmackdown.Entities;

public interface IBoard
{
    void Create(byte[] gameState);

    void Delete();

    Task<byte[]> GetGameState();
    
    void SetGameState(byte[] gameState);
}

[DataContract]
public class Board : IBoard
{
    [DataMember]
    public byte[] GameState { get; private set; } = null!;

    public void Create(byte[] gameState)
    {
        GameState = gameState;
    }
    
    public void Delete() =>
        Entity.Current.DeleteState();
    
    public Task<byte[]> GetGameState() =>
        Task.FromResult(GameState);

    public void SetGameState(byte[] gameState)
    {
        for (int i = 0; i < GameState.Length; i++)
        {
            bool isSameAdjacentBombCount = State.GetAdjacentBombCount(gameState[i]) == State.GetAdjacentBombCount(GameState[i]);
            bool isSameBombPosition = State.IsBomb(gameState[i]) == State.IsBomb(GameState[i]);
            
            if (!isSameAdjacentBombCount || !isSameBombPosition)
                throw new ArgumentException("The new game state must not have changed the immutable board data");
        }

        GameState = gameState;
    }
    
    [FunctionName(nameof(Board))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx) =>
        ctx.DispatchAsync<Board>();
}
