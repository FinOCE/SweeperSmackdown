namespace SweeperSmackdown.Bot.Services

open Microsoft.Extensions.Configuration

type ConfigurationService(configuration: IConfiguration) =
    interface IConfigurationService with
        member _.TryGetValue(key: string): string option =
            let value = configuration[key]

            if value = null then
                None
            else
                Some value
