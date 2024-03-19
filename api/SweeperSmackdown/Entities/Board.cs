using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
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

    Task<IBoard> Get();

    void SetBoard(byte[] gameState);
}

[DataContract]
public class Board : IBoard
{
    [DataMember]
    [JsonProperty("instanceId")]
    public string InstanceId { get; private set; } = null!;

    [DataMember]
    [JsonProperty("userId")]
    public string UserId { get; private set; } = null!;
    
    [DataMember]
    [JsonProperty("height")]
    public int Height { get; private set; }

    [DataMember]
    [JsonProperty("width")]
    public int Width { get; private set; }

    [DataMember]
    [JsonProperty("gameState")]
    public byte[] GameState { get; private set; } = null!;

    public void Create(
        string instanceId,
        string userId,
        int height,
        int width,
        byte[] gameState)
    {
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
    
    public Task<IBoard> Get() =>
        Task.FromResult((IBoard)this);

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
