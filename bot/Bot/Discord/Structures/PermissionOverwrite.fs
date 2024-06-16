namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type PermissionOverwrite = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: PermissionOverwriteType

    [<JsonField("allow")>]
    Allow: string

    [<JsonField("deny")>]
    Deny: string
}
