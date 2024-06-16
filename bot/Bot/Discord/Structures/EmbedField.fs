namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type EmbedField = {
    [<JsonField("name")>]
    Name: string

    [<JsonField("value")>]
    Value: string

    [<JsonField("inline")>]
    Inline: bool option
}
