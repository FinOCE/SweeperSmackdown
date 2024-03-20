using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SweeperSmackdown.Entities;

public interface IVote
{
    string InstanceId { get; }
    
    IDictionary<string, string[]> Votes { get; }

    int RequiredVotes { get; }

    string[] Choices { get; }

    void Create(string instanceId, int requiredVotes, string[] choices);

    void Delete();

    Task<IVote> Get();
    
    public void AddVote(string userId, string choice);

    public void RemoveVote(string userId);
}

[DataContract]
public class Vote : IVote
{
    [DataMember]
    [JsonProperty("instanceId")]
    public string InstanceId { get; private set; } = null!;

    [DataMember]
    [JsonProperty("votes")]
    public IDictionary<string, string[]> Votes { get; private set; } = null!;

    [DataMember]
    [JsonProperty("requiredVotes")]
    public int RequiredVotes { get; private set; }

    [DataMember]
    [JsonProperty("choices")]
    public string[] Choices { get; private set; } = null!;

    public void Create(string instanceId, int requiredVotes, string[] choices)
    {
        InstanceId = instanceId;
        Votes = new Dictionary<string, string[]>();
        RequiredVotes = requiredVotes;
        Choices = choices;

        foreach (var choice in choices)
            Votes.Add(choice, Array.Empty<string>());
    }

    public void Delete() =>
        Entity.Current.DeleteState();

    public Task<IVote> Get() =>
        Task.FromResult((IVote)this);

    public void AddVote(string userId, string choice)
    {
        if (!Choices.Contains(choice))
            throw new ArgumentException("Invalid choice provided");

        if (Votes[choice].Contains(userId))
            return;
        
        Votes[choice] = Votes[choice].Append(userId).ToArray();
    }

    public void RemoveVote(string userId)
    {
        try
        {
            var kvp = Votes.First(kvp => kvp.Value.Contains(userId));
            Votes[kvp.Key] = Votes[kvp.Key].Where(id => id != userId).ToArray();
        }
        catch (Exception)
        {
        }
    }
}
