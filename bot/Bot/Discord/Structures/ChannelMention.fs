namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type ChannelMention = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("guild_id")>]
    GuildId: string
    
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: ChannelType
    
    [<JsonField("name")>]
    Name: string
}
