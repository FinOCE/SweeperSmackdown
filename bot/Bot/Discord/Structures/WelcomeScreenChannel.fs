namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type WelcomeScreenChannel = {
    [<JsonField("channel_id")>]
    ChannelId: string

    [<JsonField("description")>]
    Description: string
    
    [<JsonField("emoji_id")>]
    EmojiId: string option

    [<JsonField("emoji_name")>]
    EmojiName: string option
}
