using System.Text.Json.Serialization;

namespace SweeperSmackdown.Structures;

public class Message<T>
{
    [JsonPropertyName("eventName")]
    public string EventName { get; }
    
    [JsonPropertyName("userId")]
    public string UserId { get; }

    [JsonPropertyName("data")]
    public T Data { get; }

    public Message(string eventName, string userId, T data)
    {
        EventName = eventName;
        UserId = userId;
        Data = data;
    }
}
