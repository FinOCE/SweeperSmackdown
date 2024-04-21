using SweeperSmackdown.Assets;
using System;

namespace SweeperSmackdown.Utils;

public static class MineUtils
{
    /// <summary>
    /// Calculate how many mines are needed on a board of a given difficulty and size.
    /// </summary>
    /// <param name="difficulty">The difficulty of the game</param>
    /// <param name="cellCount">The number of cells on the board</param>
    /// <returns>The number of mines that should be placed on the board</returns>
    /// <exception cref="ArgumentException">Occurs when an invalid difficulty is provided</exception>
    public static int CalculateMineCount(EDifficulty difficulty, int cellCount)
    {
        var ratio = difficulty switch
        {
            EDifficulty.Easy => Constants.MINE_RATIO_EASY,
            EDifficulty.Normal => Constants.MINE_RATIO_NORMAL,
            EDifficulty.Hard => Constants.MINE_RATIO_HARD,
            EDifficulty.Hell => Constants.MINE_RATIO_HELL,
            _ => throw new ArgumentException("Invalid difficulty provided")
        };

        return (int)Math.Floor(cellCount * ratio);
    }
}

public enum EDifficulty
{
    Easy,
    Normal,
    Hard,
    Hell
}
