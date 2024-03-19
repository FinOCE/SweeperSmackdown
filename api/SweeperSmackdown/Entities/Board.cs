using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Utils;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SweeperSmackdown.Entities;

public interface IBoard
{
    string InstanceId { get; }

    string UserId { get; }
    
    int Height { get; }

    int Width { get; }

    byte[] GameState { get; }

    void Create(
        string instanceId,
        string userId,
        int height,
        int width,
        byte[] gameState);

    void Delete();

    void SetBoard(byte[] gameState);
}

[DataContract]
public class Board : IBoard
{
    [DataMember]
    public string InstanceId { get; private set; } = null!;

    [DataMember]
    public string UserId { get; private set; } = null!;
    
    [DataMember]
    public int Height { get; private set; }

    [DataMember]
    public int Width { get; private set; }

    [DataMember]
    public byte[] GameState { get; private set; } = null!;

    public void Create(
        string instanceId,
        string userId,
        int height,
        int width,
        byte[] gameState)
    {
        if (height > 99 || width > 99)
            throw new ArgumentException("A board cannot have a height or width greater than 99");

        if (gameState.Length != height * width)
            throw new ArgumentException("The game state must match the size of the height and width");

        InstanceId = instanceId;
        UserId = userId;
        Height = height;
        Width = width;
        GameState = gameState;
    }

    public void Delete() =>
        Entity.Current.DeleteState();

    public void SetBoard(byte[] gameState)
    {
        if (gameState.Length != GameState.Length)
            throw new ArgumentException("The new state must be the same size as the previous state");

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
