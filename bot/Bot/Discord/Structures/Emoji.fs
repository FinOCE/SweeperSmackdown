namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type Emoji = {
    [<JsonField("id")>]
    Id: string option
    
    [<JsonField("name")>]
    Name: string option

    [<JsonField("roles")>]
    Roles: string list option

    [<JsonField("user")>]
    User: User option

    [<JsonField("require_colons")>]
    RequireColons: bool option

    [<JsonField("managed")>]
    Managed: bool option

    [<JsonField("animated")>]
    Animated: bool option

    [<JsonField("available")>]
    Available: bool option
}
