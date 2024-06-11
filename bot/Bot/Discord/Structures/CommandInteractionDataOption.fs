namespace SweeperSmackdown.Bot.Discord

open System.Text.Json.Serialization

type CommandInteractionDataOption =
    [<JsonPropertyName("name")>]
    val Name: string
    
    [<JsonPropertyName("type")>]
    val Type: ApplicationCommandType
    
    [<JsonPropertyName("value")>]
    val Value: CommandInteractionDataOptionValue option
    
    [<JsonPropertyName("options")>]
    val Options: CommandInteractionDataOption list option

    [<JsonPropertyName("focused")>]
    val Focused: bool option
    