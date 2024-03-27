using System;

namespace SweeperSmackdown.Utils;

/// <summary>
/// Utilities for working with bytes used to store game state. Each bit of the byte is used to represent a piece of
/// state. Bit 0 indicates if the state is revealed. Bit 1 indicates if the state is a bomb. Bits 2, 3, and 4 are
/// variable bits which can be used for custom game rules. Bits 5, 6, and 7 are the number of adjacent bombs in the
/// state. 
/// </summary>
public static class State
{
    public static bool ContainsBit(byte state, int offset) =>
        (1 << offset & state) == state;

    public static bool IsRevealed(byte state) =>
        ContainsBit(state, 0);
    
    public static bool IsBomb(byte state) =>
        ContainsBit(state, 1);

    public static int GetAdjacentBombCount(byte state) =>
        state >> 5;

    public static bool IsEmpty(byte state) =>
        GetAdjacentBombCount(state) == 0;

    public static bool MatchVariableBits(byte state, bool bit1, bool bit2, bool bit3) =>
        bit1 ? ContainsBit(state, 2) : true &&
        bit2 ? ContainsBit(state, 3) : true &&
        bit3 ? ContainsBit(state, 4) : true;

    public static bool IsRevealedEquivalent(byte oldState, byte newState) =>
        (byte)(oldState | 1) == newState;

    public static bool IsRevealedEquivalent(byte[] initialState, byte[] gameState)
    {
        if (initialState.Length != gameState.Length)
            return false;

        for (int i = 0; i < initialState.Length; i++)
            if (!IsRevealedEquivalent(initialState[i], gameState[i]))
                return false;

        return true;
    }

    public static byte Create(
        bool isRevealed = false,
        bool isBomb = false,
        bool bit1 = false,
        bool bit2 = false,
        bool bit3 = false,
        int adjacentBombs = 0)
    {
        if (adjacentBombs > 8 || adjacentBombs < 0)
            throw new ArgumentException("There can only be between 0 and 8 adjacent bombs");
        
        int state = 0;
        
        if (isRevealed) state += 1 << 0;
        if (isBomb) state += 1 << 1;
        if (bit1) state += 1 << 2;
        if (bit2) state += 1 << 3;
        if (bit3) state += 1 << 4;
        state += adjacentBombs << 5;

        return Convert.ToByte(state);
    }
}
