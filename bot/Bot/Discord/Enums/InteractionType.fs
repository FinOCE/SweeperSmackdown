namespace SweeperSmackdown.Bot.Discord

open System.Text.Json.Serialization
open System

type InteractionType = 
    | PING = 1
    | APPLICATION_COMMAND = 2
    | MESSAGE_COMPONENT = 3
    | APPLICATION_COMMAND_AUTOCOMPLETE = 4
    | MODAL_SUBMIT = 5


type BaseInteraction = 
    [<JsonPropertyName("id")>]
    val Id: string
