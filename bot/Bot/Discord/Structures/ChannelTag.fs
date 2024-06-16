namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type ChannelTag = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("name")>]
    Name: string
    
    [<JsonField("moderated")>]
    Moderated: bool
    
    [<JsonField("emoji_id")>]
    EmojiId: string option
    
    [<JsonField("emoji_name")>]
    EmojiName: string option
}
