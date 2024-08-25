using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder => builder
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("local.settings.json", true)
        .AddEnvironmentVariables())
    .ConfigureServices(services => { })
    .Build()
    .Run();
