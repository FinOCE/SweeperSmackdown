using SweeperSmackdown.Structures;
using System;

namespace SweeperSmackdown.Factories;

public static class GameStateFactory
{
    public static readonly int[] VALID_MODES = new int[] { 1 };

    // TODO: Refactor this into factories and builders

    public static byte[] Create(GameSettings settings) =>
        settings.Mode switch
        {
            0 => CreateNormal(settings.Height, settings.Width, settings.Mines),
            _ => throw new ArgumentException("Invalid game mode provided"),
        };

    public static byte[] CreateNormal(int height, int width, int mines)
    {
        // TODO: Implement
        return new byte[height * width];
    }
}
