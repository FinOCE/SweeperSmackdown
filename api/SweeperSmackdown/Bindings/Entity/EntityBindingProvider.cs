using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace SweeperSmackdown.Bindings.Entity;

public class EntityBindingProvider : IBindingProvider
{
    private readonly IDurableEntityClient _entityClient;

    public EntityBindingProvider(IDurableEntityClient entityClient)
    {
        _entityClient = entityClient;
    }

    public Task<IBinding> TryCreateAsync(BindingProviderContext ctx)
    {
        var attr = ctx.Parameter.GetCustomAttribute<EntityAttribute>()!;

        var type = typeof(EntityBinding<>).MakeGenericType(ctx.Parameter.ParameterType);
        var binding = Activator.CreateInstance(type, new object[] { attr, _entityClient });
        return Task.FromResult((IBinding)binding!);
    }
}
