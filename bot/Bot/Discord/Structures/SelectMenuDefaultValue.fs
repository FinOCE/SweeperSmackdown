namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type SelectMenuDefaultValue = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("type")>]
    Type: string
}
