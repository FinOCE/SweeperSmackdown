using SweeperSmackdown.Structures;
using SweeperSmackdown.Utils;
using System;

namespace SweeperSmackdown.Factories;

public static class GameStateFactory
{
    public static readonly int[] VALID_MODES = new int[] { 0 };

    public static byte[] Create(int seed, GameSettings settings)
    {
        return settings.Mode switch
        {
            0 => CreateNormal(seed, settings.Height, settings.Width, settings.Mines),
            _ => throw new ArgumentException("Invalid game mode provided"),
        };
    }

    public static byte[] CreateNormal(int seed, int height, int width, int mines)
    {
        var random = new Random(seed);
        
        // Generate mine mask
        var mineMask = new bool[height * width];
        var mineCount = 0;
        
        while (true)
        {
            var x = random.Next(height);
            var y = random.Next(width);

            if (!mineMask[x + y * width])
            {
                mineMask[x + y * width] = true;
                mineCount++;
            }

            if (mineCount == mines)
                break;
        }
        
        // Generate adjacent bomb count mask
        var adjacentMask = new int[height * width];

        for (int i = 0; i < height * width; i++)
        {
            var hugsTopEdge = i < width;
            var hugsBottomEdge = i >= height * (width - 1);
            var hugsLeftEdge = i % width == 0;
            var hugsRightEdge = i % width == width - 1;
            
            var adjacentBombs = 0;

            if (!hugsTopEdge && mineMask[i - width]) adjacentBombs++;
            if (!hugsTopEdge && !hugsRightEdge && mineMask[i - width + 1]) adjacentBombs++;
            if (!hugsRightEdge && mineMask[i + 1]) adjacentBombs++;
            if (!hugsRightEdge && !hugsBottomEdge && mineMask[i + width + 1]) adjacentBombs++;
            if (!hugsBottomEdge && mineMask[i + width]) adjacentBombs++;
            if (!hugsBottomEdge && !hugsLeftEdge && mineMask[i + width - 1]) adjacentBombs++;
            if (!hugsLeftEdge && mineMask[i - 1]) adjacentBombs++;
            if (!hugsLeftEdge && !hugsTopEdge && mineMask[i - width - 1]) adjacentBombs++;

            adjacentMask[i] = adjacentBombs;
        }

        // Map masks to state bytes
        var gameState = new byte[height * width];
        
        for (int i = 0; i < height * width; i++)
            gameState[i] = State.Create(isBomb: mineMask[i], adjacentBombs: adjacentMask[i]);

        return gameState;
    }
}
