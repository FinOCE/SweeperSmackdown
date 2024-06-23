namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type MessageActivity = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: MessageActivityType

    [<JsonField("party_id")>]
    PartyId: string option
}
