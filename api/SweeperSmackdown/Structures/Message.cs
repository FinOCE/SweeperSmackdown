using System.Text.Json.Serialization;

namespace SweeperSmackdown.Structures;

public class Message<T>
{
    [JsonPropertyName("eventName")]
    public string EventName { get; }
    
    [JsonPropertyName("userId")]
    public string UserId { get; }

    [JsonPropertyName("message")]
    public T Data { get; }

    [JsonPropertyName("content")]
    public string? Content { get; }

    public Message(string eventName, string userId, T data, string? content = null)
    {
        EventName = eventName;
        UserId = userId;
        Data = data;
        Content = content;
    }
}
