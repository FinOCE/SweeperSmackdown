using SweeperSmackdown.Functions.Entities;
using System;
using System.Text.Json.Serialization;

namespace SweeperSmackdown.Structures;

public class PlayerState
{
    [JsonPropertyName("lives")]
    public int Lives { get; set; }

    [JsonPropertyName("disabledUntil")]
    public DateTime? DisabledUntil { get; set; }

    [JsonPropertyName("boardState")]
    public string BoardState { get; set; }

    public PlayerState(int lives, DateTime? disabledUntil, byte[] boardState)
    {
        Lives = lives;
        DisabledUntil = disabledUntil;
        BoardState = new BinaryData(boardState).ToString();
    }

    public static PlayerState FromEntity(Board board) =>
        new(board.Lives, board.DisabledUntil, board.GameState);
}
