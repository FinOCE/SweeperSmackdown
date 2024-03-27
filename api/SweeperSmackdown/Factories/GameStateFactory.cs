using SweeperSmackdown.Structures;
using System;

namespace SweeperSmackdown.Factories;

public static class GameStateFactory
{
    public static readonly int[] VALID_MODES = new int[] { 0 };

    // TODO: Refactor this into factories and builders

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
        
        // TODO: Implement
        
        return new byte[height * width];
    }
}
