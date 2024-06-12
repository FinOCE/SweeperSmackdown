open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection

HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(fun services -> services.AddApplicationInsightsTelemetryWorkerService() |> ignore)
    .Build()
    .RunAsync()
|> Async.AwaitTask
|> Async.RunSynchronously
