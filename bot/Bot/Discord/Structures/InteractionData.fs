namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type InteractionData = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("name")>]
    Name: string
    
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: ApplicationCommandType
    
    [<JsonField("resolved")>]
    Resolved: ResolvedData option

    [<JsonField("options")>]
    Options: CommandInteractionDataOption list option
    
    [<JsonField("guild_id")>]
    GuildId: string option
    
    [<JsonField("target_it")>]
    TargetId: string option
}
