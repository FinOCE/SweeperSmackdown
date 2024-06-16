namespace SweeperSmackdown.Bot.Discord

open FSharp.Json
open System

type MessageCall = {
    [<JsonField("participants")>]
    Participants: string list

    [<JsonField("ended_timestamp")>]
    EndedTimestamp: DateTime option
}
