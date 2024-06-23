namespace SweeperSmackdown.Bot.Discord

open FSharp.Json
open System

type Poll = {
    [<JsonField("question")>]
    Question: PollMedia

    [<JsonField("answers")>]
    Answers: PollAnswer list

    [<JsonField("expiry")>]
    Expiry: DateTime option

    [<JsonField("allow_multiselect")>]
    AllowMultiselect: bool

    [<JsonField("layout_type", EnumValue = EnumMode.Value)>]
    LayoutType: PollLayoutType

    [<JsonField("results")>]
    Results: PollResults option
}
