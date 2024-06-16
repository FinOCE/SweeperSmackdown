namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type PingInteractionResponse = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: InteractionCallbackType
}
