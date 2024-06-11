namespace SweeperSmackdown.Bot.Discord

open System.Text.Json.Serialization

type Interaction = 
    [<JsonPropertyName("id")>]
    val Id: string

    [<JsonPropertyName("application_id")>]
    val ApplicationId: string

    [<JsonPropertyName("type")>]
    val Type: InteractionType
    
    [<JsonPropertyName("data")>]
    val Data: InteractionData option

    [<JsonPropertyName("guild")>]
    val Guild: Guild option

    [<JsonPropertyName("guild_id")>]
    val GuildId: string option

    [<JsonPropertyName("channel")>]
    val Channel: Channel option

    [<JsonPropertyName("channel_id")>]
    val ChannelId: string option
    
    [<JsonPropertyName("member")>]
    val Member: GuildMember option

    [<JsonPropertyName("user")>]
    val User: User option

    [<JsonPropertyName("token")>]
    val Token: string

    [<JsonPropertyName("version")>]
    val Version: int

    [<JsonPropertyName("message")>]
    val Message: Message option

    [<JsonPropertyName("app_permissions")>]
    val AppPermissions: string

    [<JsonPropertyName("locale")>]
    val Locale: string option

    [<JsonPropertyName("guild_locale")>]
    val GuildLocale: string option
    
    [<JsonPropertyName("entitlements")>]
    val Entitlements: Entitlement[]
    
    [<JsonPropertyName("authorizing_integration_owners")>]
    val AuthorizingIntegrationOwners: ApplicationIntegrationType
    
    [<JsonPropertyName("context")>]
    val Context: InteractionContextType option
