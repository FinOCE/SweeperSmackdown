namespace SweeperSmackdown.Bot.Discord

open System.Text.Json.Serialization

type Guild =
    [<JsonPropertyName("id")>]
    val Id: string

    [<JsonPropertyName("name")>]
    val Name: string

    [<JsonPropertyName("icon")>]
    val Icon: string option

    [<JsonPropertyName("icon_hash")>]
    val IconHash: string option

    [<JsonPropertyName("splash")>]
    val Splash: string option

    [<JsonPropertyName("discovery_splash")>]
    val DiscoverySplash: string option

    [<JsonPropertyName("owner")>]
    val Owner: bool option

    [<JsonPropertyName("owner_id")>]
    val OwnerId: string

    [<JsonPropertyName("permissions")>]
    val Permissions: string option

    [<JsonPropertyName("afk_channel_id")>]
    val AfkChannelId: string option

    [<JsonPropertyName("afk_timeout")>]
    val AfkTimeout: int

    [<JsonPropertyName("widget_enabled")>]
    val WidgetEnabled: bool option

    [<JsonPropertyName("widget_channel_id")>]
    val WidgetChannelId: string option

    [<JsonPropertyName("verification_level")>]
    val VerificationLevel: GuildVerificationLevel

    [<JsonPropertyName("default_message_notifications")>]
    val DefaultMessageNotifications: GuildMessageNotificationLevel

    [<JsonPropertyName("explicit_content_filter")>]
    val ExplicitContentFilter: GuildExplicitContentFilterLevel

    [<JsonPropertyName("roles")>]
    val Roles: Role list

    [<JsonPropertyName("emojis")>]
    val Emojis: Emoji list

    [<JsonPropertyName("features")>]
    val Featuers: string list

    [<JsonPropertyName("mfa_level")>]
    val MfaLevel: GuildMfaLevel

    [<JsonPropertyName("application_id")>]
    val ApplicationId: string option

    [<JsonPropertyName("system_channel_id")>]
    val SystemChannelId: string option

    [<JsonPropertyName("system_channel_flags")>]
    val SystemChannelFlags: int

    [<JsonPropertyName("rules_channel_id")>]
    val RulesChannelId: string option

    [<JsonPropertyName("max_presences")>]
    val MaxPresences: int option

    [<JsonPropertyName("max_members")>]
    val MaxMembers: int option

    [<JsonPropertyName("vanity_url_code")>]
    val VanityUrlCode: string option

    [<JsonPropertyName("description")>]
    val Description: string option

    [<JsonPropertyName("banner")>]
    val Banner: string option

    [<JsonPropertyName("premium_tier")>]
    val PremiumTier: GuildPremiumTier

    [<JsonPropertyName("premium_subscription_count")>]
    val PremiumSubscriptionCount: int option

    [<JsonPropertyName("preferred_locale")>]
    val PreferredLocale: string

    [<JsonPropertyName("public_updates_channel_id")>]
    val PublicUpdatesChannelId: string option

    [<JsonPropertyName("max_video_channel_users")>]
    val MaxVideoChannelUsers: int option

    [<JsonPropertyName("max_stage_video_channel_users")>]
    val MaxStageVideoChannelUsers: int option

    [<JsonPropertyName("approximate_member_count")>]
    val ApproximateMemberCount: int option

    [<JsonPropertyName("approximate_presence_count")>]
    val ApproximatePresenceCount: int option

    [<JsonPropertyName("welcome_screen")>]
    val WelcomeScreen: WelcomeScreen option

    [<JsonPropertyName("nsfw_level")>]
    val NsfwLevel: GuildNsfwLevel

    [<JsonPropertyName("stickers")>]
    val Stickers: Sticker list option

    [<JsonPropertyName("premium_progress_bar_enabled")>]
    val PremiumProgressBarEnabled: bool

    [<JsonPropertyName("safety_alerts_channel_id")>]
    val SafetyAlertsChannelId: string option
