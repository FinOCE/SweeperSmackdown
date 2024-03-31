using System;

namespace SweeperSmackdown.Utils;

/// <summary>
/// Utilities for working with bytes used to store game state. Each bit of the byte is used to represent a piece of
/// state. Bit 0 indicates if the tile has been revealed. Bit 1 indicates if the tile has been flagged. Bits 2, 3, 4,
/// and 5 represent how many adjacent bombs there are to the tile through values 0 to 8 and with 9 meaning the tile is
/// a bomb. The remaining bits 6 and 7 are variable bits that can behave differently depending on the game mode.
/// </summary>
public static class State
{
    public static int Mask(int startIndex, int? endIndex = null)
    {
        int value = 0;
        
        for (int i = startIndex; i <= endIndex; i++)
            value += 1 << i;

        return value;
    }
    public static bool ContainsBit(byte state, int offset) =>
        (Mask(offset) & state) == Mask(offset);

    public static byte Reveal(byte state) =>
        (byte)(1 << 0 | state);

    public static bool IsRevealed(byte state) =>
        ContainsBit(state, 0);

    public static byte Flag(byte state) =>
        (byte)(1 << 1 | state);

    public static byte RemoveFlag(byte state) =>
        (byte)((Mask(0) | Mask(2, 7)) & state);

    public static bool IsFlagged(byte state) =>
        ContainsBit(state, 1);

    public static int GetAdjacentBombCount(byte state) =>
        (Mask(2, 5) & state) >> 2;

    public static bool IsBomb(byte state) =>
        GetAdjacentBombCount(state) == 9;

    public static bool IsEmpty(byte state) =>
        GetAdjacentBombCount(state) == 0;

    public static bool MatchVariableBits(byte state, bool bit1, bool bit2) =>
        bit1 ? ContainsBit(state, 6) : true &&
        bit2 ? ContainsBit(state, 7) : true;

    public static bool IsRevealedEquivalent(byte oldState, byte newState) =>
        (byte)(oldState | Mask(0)) == newState;

    public static bool IsEquivalent(byte[] initialState, byte[] gameState)
    {
        if (initialState.Length != gameState.Length)
            return false;

        for (int i = 0; i < initialState.Length; i++)
        {
            var oldState = initialState[i];
            var newState = gameState[i];

            if (IsBomb(oldState) != IsBomb(newState)) return false;
            if (GetAdjacentBombCount(oldState) != GetAdjacentBombCount(newState)) return false;
        }

        return true;
    }

    public static bool IsCompleted(byte[] gameState)
    {
        for (int i = 0; i < gameState.Length; i++)
            if (!IsBomb(gameState[i]) && !IsRevealed(gameState[i]))
                return false;

        return true;
    }

    public static byte Create(
        bool isRevealed = false,
        bool isFlagged = false,
        int adjacentBombs = 0,
        bool isBomb = false,
        bool bit1 = false,
        bool bit2 = false)
    {
        if (adjacentBombs > 8 || adjacentBombs < 0)
            throw new ArgumentException("There can only be between 0 and 8 adjacent bombs");
        
        int state = 0;
        
        if (isRevealed) state += 1 << 0;
        if (isFlagged) state += 1 << 1;
        state += (isBomb ? 9 : adjacentBombs) << 2;
        if (bit1) state += 1 << 6;
        if (bit2) state += 1 << 7;

        return Convert.ToByte(state);
    }
}
