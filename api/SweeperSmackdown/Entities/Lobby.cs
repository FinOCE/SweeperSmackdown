using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Structures;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SweeperSmackdown.Entities;

public interface ILobby
{
    void Create(string[] UserIds);

    void Delete();

    Task<GameSettings> GetSettings();

    void SetSettings(GameSettings settings);

    Task<string[]> GetUserIds();

    void AddUser(string userId);
    
    void RemoveUser(string userId);

    Task<IDictionary<string, int>> GetWins();

    void AddWin(string userId);
}

[DataContract]
public class Lobby : ILobby
{
    [DataMember]
    public string[] UserIds { get; private set; } = null!;

    [DataMember]
    public IDictionary<string, int> Wins { get; private set; } = null!;

    [DataMember]
    public GameSettings Settings { get; private set; } = null!;

    public void Create(string[] userIds)
    {
        UserIds = userIds;
        Wins = new Dictionary<string, int>();
        Settings = new GameSettings();
    }

    public void Delete() =>
        Entity.Current.DeleteState();

    public Task<GameSettings> GetSettings() =>
        Task.FromResult(Settings);

    public void SetSettings(GameSettings settings)
    {
        Settings = settings;
    }

    public Task<string[]> GetUserIds() =>
        Task.FromResult(UserIds);

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

    public Task<IDictionary<string, int>> GetWins() =>
        Task.FromResult(Wins);

    public void AddWin(string userId)
    {
        if (!Wins.ContainsKey(userId))
            Wins[userId] = 1;
        else
            Wins[userId] = Wins[userId] + 1;
    }

    [FunctionName(nameof(Lobby))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx) =>
        ctx.DispatchAsync<Lobby>();
}
