using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host.Bindings;
using SweeperSmackdown.Utils;
using System;
using System.Threading.Tasks;

namespace SweeperSmackdown.Bindings.Entity;

public class EntityValueProvider<T> : IValueProvider
{
    private readonly EntityAttribute _attr;

    private readonly IDurableEntityClient _entityClient;

    public EntityValueProvider(EntityAttribute attr, IDurableEntityClient entityClient)
    {
        _attr = attr;
        _entityClient = entityClient;
    }

    public async Task<object?> GetValueAsync()
    {
        var entity = await _entityClient.ReadEntityStateAsync<T>(Id.For<T>(_attr.Id));

        return entity.EntityExists
            ? entity.EntityState
            : null;
    }

    public Type Type => typeof(object);

    public string ToInvokeString() => string.Empty;
}
