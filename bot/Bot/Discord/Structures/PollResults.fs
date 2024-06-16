namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type PollResults = {
    [<JsonField("is_finalized")>]
    IsFinalized: bool

    [<JsonField("answer_counts")>]
    AnswerCounts: PollAnswerCount list
}
