namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type Role = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("name")>]
    Name: string

    [<JsonField("color")>]
    Color: int

    [<JsonField("hoist")>]
    Hoist: bool

    [<JsonField("icon")>]
    Icon: string option

    [<JsonField("unicode_emoji")>]
    UnicodeEmoji: string option

    [<JsonField("position")>]
    Position: int

    [<JsonField("permissions")>]
    Permissions: string

    [<JsonField("managed")>]
    Managed: bool

    [<JsonField("mentionable")>]
    Mentionable: bool

    [<JsonField("tags")>]
    Tags: RoleTags option

    [<JsonField("flags")>]
    Flags: int
}
