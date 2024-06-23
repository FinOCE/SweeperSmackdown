namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type PollAnswer = {
    [<JsonField("answer_id")>]
    AnswerId: int

    [<JsonField("poll_media")>]
    PollMedia: PollMedia
}
