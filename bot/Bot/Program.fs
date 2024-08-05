open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open SweeperSmackdown.Bot.Services
open SweeperSmackdown.Bot.Commands
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
            // Services
            .AddSingleton<IConfigurationService, ConfigurationService>()
            .AddSingleton<ISigningService, Ed25519SigningService>()
            .AddSingleton<IDiscordApiService, DiscordApiService>()
            // Application commands
            .AddSingleton<PlayCommand>()
            .AddSingleton<ICommandProvider, CommandProvider>()
        |> ignore
    )
    .Build()
    .RunAsync()
|> Async.AwaitTask
|> Async.RunSynchronously
