namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type Reaction = {
    [<JsonField("count")>]
    Count: int
    
    [<JsonField("count_details")>]
    CountDetails: ReactionCountDetails

    [<JsonField("me")>]
    Me: bool

    [<JsonField("me_burst")>]
    MeBurst: bool

    [<JsonField("emoji")>]
    Emoji: Emoji

    [<JsonField("burst_colors")>]
    BurstColors: int list
}
