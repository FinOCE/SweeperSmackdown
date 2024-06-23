namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type ApplicationIntegrationTypeConfiguration = {
    [<JsonField("oauth2_install_params")>]
    Oauth2InstallParams: OAuth2InstallParams option
}
