namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type PollAnswerCount = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("count")>]
    Count: int

    [<JsonField("me_voted")>]
    MeVoted: bool
}
