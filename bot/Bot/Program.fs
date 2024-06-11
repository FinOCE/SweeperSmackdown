open Microsoft.Extensions.Hosting

HostBuilder()
    .ConfigureFunctionsWebApplication()
    .Build()
    .Run()
