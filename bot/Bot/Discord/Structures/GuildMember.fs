namespace SweeperSmackdown.Bot.Discord

open FSharp.Json
open System

type GuildMember = {
    [<JsonField("user")>]
    User: User option
    
    [<JsonField("nick")>]
    Nick: string option

    [<JsonField("avatar")>]
    Avatar: string option

    [<JsonField("roles")>]
    Roles: string list

    [<JsonField("joined_at")>]
    JoinedAt: DateTime option

    [<JsonField("premium_since")>]
    PremiumSince: DateTime option

    [<JsonField("deaf")>]
    Deaf: bool

    [<JsonField("mute")>]
    Mute: bool

    [<JsonField("flags")>]
    Flags: int

    [<JsonField("pending")>]
    Pending: bool option

    [<JsonField("permissions")>]
    Permissions: string option

    [<JsonField("communication_disabled_until")>]
    CommunicationDisabledUntil: DateTime option

    [<JsonField("avatar_decoration_metadata")>]
    AvatarDecorationData: AvatarDecorationData option
}
