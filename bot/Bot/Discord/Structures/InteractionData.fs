namespace SweeperSmackdown.Bot.Discord

open System.Text.Json.Serialization

type InteractionData =
    [<JsonPropertyName("id")>]
    val Id: string
    
    [<JsonPropertyName("name")>]
    val Name: string
    
    [<JsonPropertyName("type")>]
    val Type: ApplicationCommandType
    
    [<JsonPropertyName("resolved")>]
    val Resolved: ResolvedData option

    [<JsonPropertyName("options")>]
    val Options: CommandInteractionDataOption list option
    
    [<JsonPropertyName("guild_id")>]
    val GuildId: string option
    
    [<JsonPropertyName("target_it")>]
    val TargetId: string option
