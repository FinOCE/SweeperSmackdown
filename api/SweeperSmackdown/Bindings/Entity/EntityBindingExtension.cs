using Microsoft.Azure.WebJobs;

namespace SweeperSmackdown.Bindings.Entity;

public static class EntityBindingExtension
{
    public static IWebJobsBuilder AddEntityBinding(this IWebJobsBuilder builder)
    {
        builder.AddExtension<EntityExtensionProvider>();
        return builder;
    }
}
