namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type WelcomeScreen = {
    [<JsonField("description")>]
    Description: string option

    [<JsonField("welcome_channels")>]
    WelcomeChannels: WelcomeScreenChannel list
}
