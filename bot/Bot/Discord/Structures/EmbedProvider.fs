namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type EmbedProvider = {
    [<JsonField("name")>]
    Name: string option

    [<JsonField("url")>]
    Url: string option
}
