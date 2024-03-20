﻿using Microsoft.Azure.WebJobs;
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

    void Create((string InstanceId, int RequiredVotes, string[] Choices) args);

    void Delete();

    Task<IVote> Get();
    
    public void AddVote((string UserId, string Choice) args);

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

    public void Create((string InstanceId, int RequiredVotes, string[] Choices) args)
    {
        InstanceId = args.InstanceId;
        Votes = new Dictionary<string, string[]>();
        RequiredVotes = args.RequiredVotes;
        Choices = args.Choices;
        
        foreach (var choice in args.Choices)
            Votes.Add(choice, Array.Empty<string>());
    }

    public void Delete() =>
        Entity.Current.DeleteState();

    public Task<IVote> Get() =>
        Task.FromResult((IVote)this);

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

    [FunctionName(nameof(Vote))]
    public static Task Run([EntityTrigger] IDurableEntityContext ctx) =>
        ctx.DispatchAsync<Vote>();
}
