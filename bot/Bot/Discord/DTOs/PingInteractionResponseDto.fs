namespace SweeperSmackdown.Bot.Discord

open System.Text.Json.Serialization

type PingInteractionResponseDto() = 
    [<JsonPropertyName("type")>]
    member val Type: InteractionCallbackType = InteractionCallbackType.PONG with get, set
