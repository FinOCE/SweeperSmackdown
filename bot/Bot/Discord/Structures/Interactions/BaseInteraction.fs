namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type BaseInteraction = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: InteractionType
}
