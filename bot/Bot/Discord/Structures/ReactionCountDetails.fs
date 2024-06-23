namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type ReactionCountDetails = {
    [<JsonField("burst")>]
    Burst: int

    [<JsonField("normal")>]
    Normal: int
}
