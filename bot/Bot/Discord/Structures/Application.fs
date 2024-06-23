namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type Application = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("name")>]
    Name: string

    [<JsonField("icon")>]
    Icon: string option

    [<JsonField("description")>]
    Description: string

    [<JsonField("rpc_origins")>]
    RpcOrigins: string list option

    [<JsonField("bot_public")>]
    BotPublic: bool

    [<JsonField("bot_require_code_grant")>]
    BotRequireCodeGrant: bool

    [<JsonField("bot")>]
    Bot: User option

    [<JsonField("terms_of_Service_url")>]
    TermsOfServiceUrl: string option

    [<JsonField("privacy_policy_url")>]
    PrivacyPolicyUrl: string option

    [<JsonField("owner")>]
    Owner: User option

    [<JsonField("verify_key")>]
    VerifyKey: string

    [<JsonField("team")>]
    Team: Team option

    [<JsonField("guild_id")>]
    GuildId: string option

    [<JsonField("guild")>]
    Guild: Guild option

    [<JsonField("primary_sku_id")>]
    PrimarySkuId: string option

    [<JsonField("slug")>]
    Slug: string option

    [<JsonField("cover_image")>]
    CoverImage: string option

    [<JsonField("flags")>]
    Flags: int option

    [<JsonField("approximate_guild_count")>]
    ApproximateGuildCount: int option

    [<JsonField("redirect_uris")>]
    RedirectUris: string list option

    [<JsonField("interactions_endpoint_url")>]
    InteractionsEndpointUrl: string option

    [<JsonField("role_connections_verification_url")>]
    RoleConnectionsVerificationUrl: string option

    [<JsonField("tags")>]
    Tags: string list option

    [<JsonField("install_params")>]
    InstallParams: OAuth2InstallParams option

    [<JsonField("integration_types_config")>]
    IntegrationTypesConfig: Map<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration> option

    [<JsonField("custom_install_url")>]
    CustomInstallUrl: string option
}
