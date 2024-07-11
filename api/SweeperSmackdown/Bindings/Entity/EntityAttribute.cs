using Microsoft.Azure.WebJobs.Description;
using System;

namespace SweeperSmackdown.Bindings.Entity;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
[Binding]
public class EntityAttribute : Attribute
{
    [AutoResolve]
    public string Id { get; set; }

    public EntityAttribute(string id)
    {
        Id = id;
    }
}
