namespace SweeperSmackdown.Bot.Services

open Microsoft.Extensions.Configuration

type ConfigurationService(configuration: IConfiguration) =
    interface IConfigurationService with
        member _.TryGetValue key =
            match configuration[key] with
            | null -> None
            | value -> Some value

        member _.GetValue key =
            match configuration[key] with
            | null -> failwith $"Failed to fetch '{key}' from environment"
            | value -> value
