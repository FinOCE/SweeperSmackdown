open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open SweeperSmackdown.Bot.Services

HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(fun services -> 
        services
            .AddApplicationInsightsTelemetryWorkerService()
            .AddSingleton<IConfigurationService, ConfigurationService>()
            .AddSingleton<ISigningService, Ed25519SigningService>()
        |> ignore
    )
    .Build()
    .RunAsync()
|> Async.AwaitTask
|> Async.RunSynchronously
