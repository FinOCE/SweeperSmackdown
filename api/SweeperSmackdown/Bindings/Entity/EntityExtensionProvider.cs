using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.ContextImplementations;
using Microsoft.Azure.WebJobs.Host.Config;

namespace SweeperSmackdown.Bindings.Entity;

public class EntityExtensionProvider : IExtensionConfigProvider
{
    private readonly IDurableEntityClient _entityClient;

    public EntityExtensionProvider(IDurableClientFactory durableClientFactory)
    {
        _entityClient = durableClientFactory.CreateClient();
    }

    public void Initialize(ExtensionConfigContext ctx)
    {
        ctx.AddBindingRule<EntityAttribute>().Bind(new EntityBindingProvider(_entityClient));
    }
}
