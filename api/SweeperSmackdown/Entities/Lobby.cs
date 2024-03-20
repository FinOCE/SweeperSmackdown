using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using SweeperSmackdown.Structures;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SweeperSmackdown.Entities;

public interface ILobby
{
    void Create((string InstanceId, string[] UserIds) args);

    void Create((string InstanceId, string[] UserIds, GameSettings Settings) args);

    void Delete();
    
    Task<Lobby> Get();

    void AddUser(string userId);
    
    void RemoveUser(string userId);

    void SetSettings(GameSettings settings);

    void SetStatus(ELobbyStatus status);
}

[DataContract]
public class Lobby : ILobby
{
    [DataMember]
    [JsonProperty("instanceId")]
    [JsonPropertyName("instanceId")]
    public string InstanceId { get; private set; } = null!;

    [DataMember]
    [JsonProperty("userIds")]
    [JsonPropertyName("userIds")]
    public string[] UserIds { get; private set; } = null!;

    [DataMember]
    [JsonProperty("settings")]
    [JsonPropertyName("settings")]
    public GameSettings Settings { get; private set; } = null!;

    [DataMember]
    [JsonProperty("status")]
    [JsonPropertyName("status")]
    public ELobbyStatus Status { get; private set; }

    public void Create((string InstanceId, string[] UserIds) args)
    {
        InstanceId = args.InstanceId;
        UserIds = args.UserIds;
        Settings = new GameSettings();
        Status = ELobbyStatus.Setup;
    }

    public void Create((string InstanceId, string[] UserIds, GameSettings Settings) args)
    {
        InstanceId = args.InstanceId;
        UserIds = args.UserIds;
        Settings = args.Settings;
        Status = ELobbyStatus.Setup;
    }

    public void Delete() =>
        Entity.Current.DeleteState();
    
    public Task<Lobby> Get() =>
        Task.FromResult(this);

    public void AddUser(string userId)
    {
        UserIds = UserIds
            .Append(userId)
            .Distinct()
            .ToArray();
    }

    public void RemoveUser(string userId)
    {
        UserIds = UserIds
            .Where(id => id != userId)
            .ToArray();
    }

    public void SetSettings(GameSettings settings)
    {
        Settings = settings;
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
