using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SweeperSmackdown.Assets;
using SweeperSmackdown.Functions.Orchestrators;
using SweeperSmackdown.Utils;
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

    public void AddVote((string UserId, string Choice, string LobbyId, IDurableOrchestrationClient OrchestrationClient) args);

    public void RemoveVote((string UserId, string LobbyId, IDurableOrchestrationClient OrchestrationClient) args);

    public Task<int> GetRequiredVotes();

    public void SetRequiredVotes((int RequiredVotes, string LobbyId, IDurableOrchestrationClient OrchestrationClient) args);

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

    public void AddVote((string UserId, string Choice, string LobbyId, IDurableOrchestrationClient OrchestrationClient) args)
    {
        if (!Choices.Contains(args.Choice))
            throw new ArgumentException("Invalid choice provided");

        if (Votes[args.Choice].Contains(args.UserId))
            return;
        
        Votes[args.Choice] = Votes[args.Choice].Append(args.UserId).ToArray();

        if (Votes[args.Choice].Length >= RequiredVotes)
            args.OrchestrationClient.RaiseEventAsync(
                Id.ForInstance(nameof(TimerOrchestratorFunction), args.LobbyId),
                DurableEvents.START_TIMER);
    }

    public void RemoveVote((string UserId, string LobbyId, IDurableOrchestrationClient OrchestrationClient) args)
    {
        try
        {
            var kvp = Votes.First(kvp => kvp.Value.Contains(args.UserId));
            Votes[kvp.Key] = Votes[kvp.Key].Where(id => id != args.UserId).ToArray();
        }
        catch (Exception)
        {
            return;
        }

        if (Votes.All(vote => vote.Value.Length < RequiredVotes))
            args.OrchestrationClient.RaiseEventAsync(
                Id.ForInstance(nameof(TimerOrchestratorFunction), args.LobbyId),
                DurableEvents.RESET_TIMER);
    }

    public Task<int> GetRequiredVotes() =>
        Task.FromResult(RequiredVotes);

    public void SetRequiredVotes((int RequiredVotes, string LobbyId, IDurableOrchestrationClient OrchestrationClient) args)
    {
        var oldRequiredVotes = RequiredVotes;
        RequiredVotes = args.RequiredVotes;

        if (Votes.Any(vote => vote.Value.Length >= RequiredVotes) && Votes.All(vote => vote.Value.Length < oldRequiredVotes))
            args.OrchestrationClient.RaiseEventAsync(
                Id.ForInstance(nameof(TimerOrchestratorFunction), args.LobbyId),
                DurableEvents.START_TIMER);

        if (Votes.All(vote => vote.Value.Length < RequiredVotes))
            args.OrchestrationClient.RaiseEventAsync(
                Id.ForInstance(nameof(TimerOrchestratorFunction), args.LobbyId),
                DurableEvents.RESET_TIMER);
    }

    public Task<string[]> GetChoices() =>
        Task.FromResult(Choices);

    [FunctionName(nameof(Vote))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx) =>
        ctx.DispatchAsync<Vote>();
}
