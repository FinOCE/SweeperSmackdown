namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type Team = {
    [<JsonField("icon")>]
    Icon: string option
    
    [<JsonField("id")>]
    Id: string

    [<JsonField("members")>]
    Members: TeamMember list

    [<JsonField("name")>]
    Name: string

    [<JsonField("owner_user_id")>]
    OwnerUserId: string
}
