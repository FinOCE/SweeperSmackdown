using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SweeperSmackdown.Entities;

public interface ILobby
{
    string InstanceId { get; }
    
    string[] UserIds { get; }

    int? Lifetime { get; }
    
    int? Mode { get; }
    
    int? Height { get; }

    int? Width { get; }

    ELobbyStatus Status { get; }

    void Create(string instanceId, string[] userIds);

    void Delete();

    void AddUser(string userId);

    void RemoveUser(string userId);

    void SetLifetime(int lifetime);

    void SetMode(int mode);

    void SetHeight(int height);

    void SetWidth(int width);

    void SetStatus(ELobbyStatus status);
}

[DataContract]
public class Lobby : ILobby
{
    [DataMember]
    [JsonProperty("instanceId")]
    public string InstanceId { get; private set; } = null!;

    [DataMember]
    [JsonProperty("userIds")]
    public string[] UserIds { get; private set; } = null!;

    [DataMember]
    [JsonProperty("lifetime")]
    public int? Lifetime { get; private set; }

    [DataMember]
    [JsonProperty("mode")]
    public int? Mode { get; private set; }

    [DataMember]
    [JsonProperty("height")]
    public int? Height { get; private set; }

    [DataMember]
    [JsonProperty("width")]
    public int? Width { get; private set; }

    [DataMember]
    [JsonProperty("status")]
    public ELobbyStatus Status { get; private set; }

    public void Create(string instanceId, string[] userIds)
    {
        InstanceId = instanceId;
        UserIds = userIds;
        Mode = null;
        Height = null;
        Width = null;
        Status = ELobbyStatus.Setup;
    }

    public void Delete() =>
        Entity.Current.DeleteState();

    public void AddUser(string userId)
    {
        if (!UserIds.Contains(userId))
            UserIds = UserIds.Append(userId).ToArray();
    }

    public void RemoveUser(string userId)
    {
        UserIds = UserIds.Where(id => id != userId).ToArray();
    }

    public void SetLifetime(int lifetime)
    {
        Lifetime = lifetime;
    }

    public void SetMode(int mode)
    {
        Mode = mode;
    }

    public void SetHeight(int height)
    {
        Height = height;
    }

    public void SetWidth(int width)
    {
        Width = width;
    }

    public void SetStatus(ELobbyStatus status)
    {
        Status = status;
    }

    [FunctionName(nameof(Lobby))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx) =>
        ctx.DispatchAsync<Lobby>();
}

public enum ELobbyStatus
{
    Setup,
    Countdown,
    Playing,
    Celebration
}
