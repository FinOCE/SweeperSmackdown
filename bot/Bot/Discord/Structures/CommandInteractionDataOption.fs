namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type CommandInteractionDataOption = {
    [<JsonField("name")>]
    Name: string
    
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: ApplicationCommandType
    
    [<JsonField("value")>]
    Value: CommandInteractionDataOptionValue option
    
    [<JsonField("options")>]
    Options: CommandInteractionDataOption list option

    [<JsonField("focused")>]
    Focused: bool option
}
