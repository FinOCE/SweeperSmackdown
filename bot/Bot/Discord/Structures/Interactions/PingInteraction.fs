namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type PingInteraction = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: InteractionType
}
