namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type PollMedia = {
    [<JsonField("text")>]
    Text: string option

    [<JsonField("emoji")>]
    Emoji: Emoji option
}
