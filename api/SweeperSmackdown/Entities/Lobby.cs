using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
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

    void Create(string instanceId, string[] userIds);

    void Delete();

    void AddUser(string userId);

    void RemoveUser(string userId);

    void SetLifetime(int lifetime);

    void SetMode(int mode);

    void SetHeight(int height);

    void SetWidth(int width);
}

[DataContract]
public class Lobby : ILobby
{
    [DataMember]
    public string InstanceId { get; private set; } = null!;

    [DataMember]
    public string[] UserIds { get; private set; } = null!;

    [DataMember]
    public int? Lifetime { get; private set; }

    [DataMember]
    public int? Mode { get; private set; }

    [DataMember]
    public int? Height { get; private set; }

    [DataMember]
    public int? Width { get; private set; }

    public void Create(string instanceId, string[] userIds)
    {
        InstanceId = instanceId;
        UserIds = userIds;
        Mode = null;
        Height = null;
        Width = null;
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

    [FunctionName(nameof(Lobby))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx) =>
        ctx.DispatchAsync<Lobby>();
}
