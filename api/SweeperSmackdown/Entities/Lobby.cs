﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SweeperSmackdown.Entities;

public interface ILobby
{
    void Create((string InstanceId, string[] UserIds) args);

    void Create((string InstanceId, string[] UserIds, int? Lifetime, int? Mode, int? Height, int? Width) args);

    void Delete();
    
    Task<Lobby> Get();

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
    [JsonPropertyName("instanceId")]
    public string InstanceId { get; private set; } = null!;

    [DataMember]
    [JsonProperty("userIds")]
    [JsonPropertyName("userIds")]
    public string[] UserIds { get; private set; } = null!;

    [DataMember]
    [JsonProperty("lifetime")]
    [JsonPropertyName("lifetime")]
    public int? Lifetime { get; private set; }

    [DataMember]
    [JsonProperty("mode")]
    [JsonPropertyName("mode")]
    public int? Mode { get; private set; }

    [DataMember]
    [JsonProperty("height")]
    [JsonPropertyName("height")]
    public int? Height { get; private set; }

    [DataMember]
    [JsonProperty("width")]
    [JsonPropertyName("width")]
    public int? Width { get; private set; }

    [DataMember]
    [JsonProperty("status")]
    [JsonPropertyName("status")]
    public ELobbyStatus Status { get; private set; }

    public void Create((string InstanceId, string[] UserIds) args)
    {
        InstanceId = args.InstanceId;
        UserIds = args.UserIds;
        Lifetime = null;
        Mode = null;
        Height = null;
        Width = null;
        Status = ELobbyStatus.Setup;
    }

    public void Create((string InstanceId, string[] UserIds, int? Lifetime, int? Mode, int? Height, int? Width) args)
    {
        InstanceId = args.InstanceId;
        UserIds = args.UserIds;
        Lifetime = args.Lifetime;
        Mode = args.Mode;
        Height = args.Height;
        Width = args.Width;
        Status = ELobbyStatus.Setup;
    }

    public void Delete() =>
        Entity.Current.DeleteState();
    
    public Task<Lobby> Get() =>
        Task.FromResult(this);

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
