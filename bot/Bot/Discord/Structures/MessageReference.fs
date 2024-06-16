namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type MessageReference = {
    [<JsonField("message_id")>]
    MessageId: string option

    [<JsonField("channel_id")>]
    ChannelId: string option

    [<JsonField("guild_id")>]
    GuildId: string option

    [<JsonField("fail_if_not_exists")>]
    FailIfNotExists: bool option
}
