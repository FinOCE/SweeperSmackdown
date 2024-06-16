namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type MessageInteraction = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("type")>]
    Type: InteractionType

    [<JsonField("name")>]
    Name: string

    [<JsonField("user")>]
    User: User

    [<JsonField("member")>]
    Member: GuildMember option
}
