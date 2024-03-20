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
    void Create((string InstanceId, string UserId, int Height, int Width, byte[] GameState) args);

    void Delete();

    Task<Board> Get();

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

    public void Create((string InstanceId, string UserId, int Height, int Width, byte[] GameState) args)
    {
        if (args.GameState.Length != args.Height * args.Width)
            throw new ArgumentException("The game state must match the size of the height and width");

        InstanceId = args.InstanceId;
        UserId = args.UserId;
        Height = args.Height;
        Width = args.Width;
        GameState = args.GameState;
    }

    public void Delete() =>
        Entity.Current.DeleteState();
    
    public Task<Board> Get() =>
        Task.FromResult(this);

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
