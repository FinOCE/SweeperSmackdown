namespace SweeperSmackdown.Bot.Discord

open System.Text.Json.Serialization

type WelcomeScreenChannel =
    [<JsonPropertyName("channel_id")>]
    val ChannelId: string

    [<JsonPropertyName("description")>]
    val Description: string
    
    [<JsonPropertyName("emoji_id")>]
    val EmojiId: string option

    [<JsonPropertyName("emoji_name")>]
    val EmojiName: string option
