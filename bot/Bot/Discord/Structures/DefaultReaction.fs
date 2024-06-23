namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type DefaultReaction = {
    [<JsonField("emoji_id")>]
    EmojiId: string option
    
    [<JsonField("emoji_name")>]
    EmojiName: string option
}
