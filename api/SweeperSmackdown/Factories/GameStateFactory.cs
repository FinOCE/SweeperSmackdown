using System;

namespace SweeperSmackdown.Factories;

public static class GameStateFactory
{
    public static byte[] Create(int mode, int height, int width)
    {
        return mode switch
        {
            1 => CreateNormal(height, width),
            _ => throw new ArgumentException("Invalid game mode provided"),
        };
    }

    public static byte[] CreateNormal(int height, int width)
    {
        // TODO: Implement
        return Array.Empty<byte>();
    }
}
