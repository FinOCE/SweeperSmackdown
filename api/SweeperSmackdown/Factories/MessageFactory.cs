using SweeperSmackdown.Structures;
using System.Text.Json;

namespace SweeperSmackdown.Factories;

public static class MessageFactory
{
    public static string Create<T>(string eventName, T data, string? content = null) =>
        JsonSerializer.Serialize(new Message<T>(eventName, "SYSTEM", data, content));

    public static string Create<T>(string eventName, string userId, T data, string? content = null) =>
        JsonSerializer.Serialize(new Message<T>(eventName, userId, data, content));
}
