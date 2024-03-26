using SweeperSmackdown.Structures;
using System.Text.Json;

namespace SweeperSmackdown.Factories;

public static class MessageFactory
{
    public static string Create<T>(string eventName, string userId, T data) =>
        JsonSerializer.Serialize(new Message<T>(eventName, userId, data));
}
