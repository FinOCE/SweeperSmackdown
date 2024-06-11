namespace SweeperSmackdown.Bot.Discord

open System.Text.Json.Serialization

type WelcomeScreen =
    [<JsonPropertyName("description")>]
    val Description: string option

    [<JsonPropertyName("welcome_channels")>]
    val WelcomeChannels: WelcomeScreenChannel list
