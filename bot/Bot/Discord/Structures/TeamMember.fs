namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type TeamMember = {
    [<JsonField("membership_state", EnumValue = EnumMode.Value)>]
    MembershipState: TeamMembershipState
    
    [<JsonField("team_id")>]
    TeamId: string

    [<JsonField("user")>]
    User: User

    [<JsonField("role")>]
    Role: string
}
