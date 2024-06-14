namespace SweeperSmackdown.Bot.Discord

open System.Text.Json.Serialization

type Interaction() =
    [<DefaultValue>]
    [<JsonPropertyName("id")>]
    val mutable Id: string
    
    [<DefaultValue>]
    [<JsonPropertyName("application_id")>]
    val mutable ApplicationId: string
    
    [<DefaultValue>]
    [<JsonPropertyName("type")>]
    val mutable Type: InteractionType
    
    [<DefaultValue>]
    [<JsonPropertyName("data")>]
    val mutable Data: InteractionData option
    
    [<DefaultValue>]
    [<JsonPropertyName("guild")>]
    val mutable Guild: Guild option
    
    [<DefaultValue>]
    [<JsonPropertyName("guild_id")>]
    val mutable GuildId: string option
    
    [<DefaultValue>]
    [<JsonPropertyName("channel")>]
    val mutable Channel: Channel option
    
    [<DefaultValue>]
    [<JsonPropertyName("channel_id")>]
    val mutable ChannelId: string option
    
    [<DefaultValue>]
    [<JsonPropertyName("member")>]
    val mutable Member: GuildMember option
    
    [<DefaultValue>]
    [<JsonPropertyName("user")>]
    val mutable User: User option
    
    [<DefaultValue>]
    [<JsonPropertyName("token")>]
    val mutable Token: string
    
    [<DefaultValue>]
    [<JsonPropertyName("version")>]
    val mutable Version: int
    
    [<DefaultValue>]
    [<JsonPropertyName("message")>]
    val mutable Message: Message option
    
    [<DefaultValue>]
    [<JsonPropertyName("app_permissions")>]
    val mutable AppPermissions: string
    
    [<DefaultValue>]
    [<JsonPropertyName("locale")>]
    val mutable Locale: string option
    
    [<DefaultValue>]
    [<JsonPropertyName("guild_locale")>]
    val mutable GuildLocale: string option
    
    [<DefaultValue>]
    [<JsonPropertyName("entitlements")>]
    val mutable Entitlements: Entitlement list
    
    [<DefaultValue>]
    [<JsonPropertyName("authorizing_integration_owners")>]
    val mutable AuthorizingIntegrationOwners: ApplicationIntegrationType
    
    [<DefaultValue>]
    [<JsonPropertyName("context")>]
    val mutable Context: InteractionContextType option
