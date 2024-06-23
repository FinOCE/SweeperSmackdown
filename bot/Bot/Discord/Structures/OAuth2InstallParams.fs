namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type OAuth2InstallParams = {
    [<JsonField("scopes")>]
    Scopes: string list

    [<JsonField("permissions")>]
    Permissions: string
}
