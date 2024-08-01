open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open SweeperSmackdown.Bot.Services
open System.IO

HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(fun builder ->
        builder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", true)
            .AddEnvironmentVariables()
        |> ignore
    )
    .ConfigureServices(fun services -> 
        services
            .AddApplicationInsightsTelemetryWorkerService()
            .AddSingleton<IConfigurationService, ConfigurationService>()
            .AddSingleton<IDiscordApiService, DiscordApiService>()
            .AddSingleton<ISigningService, Ed25519SigningService>()
        |> ignore
    )
    .Build()
    .RunAsync()
|> Async.AwaitTask
|> Async.RunSynchronously
