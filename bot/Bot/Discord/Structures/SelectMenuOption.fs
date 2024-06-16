namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type SelectMenuOption = {
    [<JsonField("label")>]
    Label: string

    [<JsonField("value")>]
    Value: string

    [<JsonField("description")>]
    Description: string option

    [<JsonField("emoji")>]
    Emoji: Emoji option

    [<JsonField("default")>]
    Default: bool option
}
