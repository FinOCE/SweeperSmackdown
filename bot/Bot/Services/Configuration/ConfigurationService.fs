namespace SweeperSmackdown.Bot.Services

open Microsoft.Extensions.Configuration

type ConfigurationService(configuration: IConfiguration) =
    interface IConfigurationService with
        member _.TryGetValue(key: string): string option =
            match configuration[key] with
            | null -> None
            | value -> Some value
