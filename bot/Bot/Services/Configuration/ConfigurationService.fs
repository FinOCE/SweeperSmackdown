namespace SweeperSmackdown.Bot.Services

open Microsoft.Extensions.Configuration
open System.Configuration

type ConfigurationService(configuration: IConfiguration) =
    interface IConfigurationService with
        member _.ReadOrThrow(key: string): string =
            let value = configuration[key]

            if value = null then
                raise (SettingsPropertyNotFoundException key)
            else
                value
