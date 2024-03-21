using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SweeperSmackdown.Entities;

public interface IVote
{
    void Create((int RequiredVotes, string[] Choices) args);

    void Delete();
    
    Task<IDictionary<string, string[]>> GetVotes();
    
    public void AddVote((string UserId, string Choice) args);

    public void RemoveVote(string userId);

    public Task<int> GetRequiredVotes();

    public void SetRequiredVotes(int requiredVotes);

    public Task<string[]> GetChoices();
}

[DataContract]
public class Vote : IVote
{
    [DataMember]
    public IDictionary<string, string[]> Votes { get; private set; } = null!;

    [DataMember]
    public int RequiredVotes { get; private set; }

    [DataMember]
    public string[] Choices { get; private set; } = null!;

    public void Create((int RequiredVotes, string[] Choices) args)
    {
        Votes = new Dictionary<string, string[]>();
        RequiredVotes = args.RequiredVotes;
        Choices = args.Choices;
        
        foreach (var choice in args.Choices)
            Votes.Add(choice, Array.Empty<string>());
    }

    public void Delete() =>
        Entity.Current.DeleteState();
    
    public Task<IDictionary<string, string[]>> GetVotes() =>
        Task.FromResult(Votes);

    public void AddVote((string UserId, string Choice) args)
    {
        if (!Choices.Contains(args.Choice))
            throw new ArgumentException("Invalid choice provided");

        if (Votes[args.Choice].Contains(args.UserId))
            return;
        
        Votes[args.Choice] = Votes[args.Choice].Append(args.UserId).ToArray();
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

    public Task<int> GetRequiredVotes() =>
        Task.FromResult(RequiredVotes);

    public void SetRequiredVotes(int requiredVotes) =>
        RequiredVotes = requiredVotes;

    public Task<string[]> GetChoices() =>
        Task.FromResult(Choices);

    [FunctionName(nameof(Vote))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx) =>
        ctx.DispatchAsync<Vote>();
}
