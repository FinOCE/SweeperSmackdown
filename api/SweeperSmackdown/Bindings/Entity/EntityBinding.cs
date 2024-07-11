using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using System.Threading.Tasks;

namespace SweeperSmackdown.Bindings.Entity;

public class EntityBinding<T> : IBinding
{
    private readonly EntityAttribute _attr;

    private readonly IDurableEntityClient _entityClient;

    public EntityBinding(EntityAttribute attr, IDurableEntityClient entityClient)
    {
        _attr = attr;
        _entityClient = entityClient;
    }

    public Task<IValueProvider> BindAsync(BindingContext ctx)
    {
        return Task.FromResult<IValueProvider>(new EntityValueProvider<T>(_attr, _entityClient));
    }

    public bool FromAttribute => true;

    public Task<IValueProvider>? BindAsync(object value, ValueBindingContext ctx)
    {
        return null;
    }

    public ParameterDescriptor ToParameterDescriptor()
    {
        return new ParameterDescriptor();
    }
}
