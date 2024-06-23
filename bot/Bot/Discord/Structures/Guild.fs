namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type Guild = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("name")>]
    Name: string

    [<JsonField("icon")>]
    Icon: string option

    [<JsonField("icon_hash")>]
    IconHash: string option

    [<JsonField("splash")>]
    Splash: string option

    [<JsonField("discovery_splash")>]
    DiscoverySplash: string option

    [<JsonField("owner")>]
    Owner: bool option

    [<JsonField("owner_id")>]
    OwnerId: string

    [<JsonField("permissions")>]
    Permissions: string option

    [<JsonField("afk_channel_id")>]
    AfkChannelId: string option

    [<JsonField("afk_timeout")>]
    AfkTimeout: int

    [<JsonField("widget_enabled")>]
    WidgetEnabled: bool option

    [<JsonField("widget_channel_id")>]
    WidgetChannelId: string option

    [<JsonField("verification_level", EnumValue = EnumMode.Value)>]
    VerificationLevel: GuildVerificationLevel

    [<JsonField("default_message_notifications", EnumValue = EnumMode.Value)>]
    DefaultMessageNotifications: GuildMessageNotificationLevel

    [<JsonField("explicit_content_filter", EnumValue = EnumMode.Value)>]
    ExplicitContentFilter: GuildExplicitContentFilterLevel

    [<JsonField("roles")>]
    Roles: Role list

    [<JsonField("emojis")>]
    Emojis: Emoji list

    [<JsonField("features")>]
    Featuers: string list

    [<JsonField("mfa_level", EnumValue = EnumMode.Value)>]
    MfaLevel: GuildMfaLevel

    [<JsonField("application_id")>]
    ApplicationId: string option

    [<JsonField("system_channel_id")>]
    SystemChannelId: string option

    [<JsonField("system_channel_flags")>]
    SystemChannelFlags: int

    [<JsonField("rules_channel_id")>]
    RulesChannelId: string option

    [<JsonField("max_presences")>]
    MaxPresences: int option

    [<JsonField("max_members")>]
    MaxMembers: int option

    [<JsonField("vanity_url_code")>]
    VanityUrlCode: string option

    [<JsonField("description")>]
    Description: string option

    [<JsonField("banner")>]
    Banner: string option

    [<JsonField("premium_tier", EnumValue = EnumMode.Value)>]
    PremiumTier: GuildPremiumTier

    [<JsonField("premium_subscription_count")>]
    PremiumSubscriptionCount: int option

    [<JsonField("preferred_locale")>]
    PreferredLocale: string

    [<JsonField("public_updates_channel_id")>]
    PublicUpdatesChannelId: string option

    [<JsonField("max_video_channel_users")>]
    MaxVideoChannelUsers: int option

    [<JsonField("max_stage_video_channel_users")>]
    MaxStageVideoChannelUsers: int option

    [<JsonField("approximate_member_count")>]
    ApproximateMemberCount: int option

    [<JsonField("approximate_presence_count")>]
    ApproximatePresenceCount: int option

    [<JsonField("welcome_screen")>]
    WelcomeScreen: WelcomeScreen option

    [<JsonField("nsfw_level", EnumValue = EnumMode.Value)>]
    NsfwLevel: GuildNsfwLevel

    [<JsonField("stickers")>]
    Stickers: Sticker list option

    [<JsonField("premium_progress_bar_enabled")>]
    PremiumProgressBarEnabled: bool

    [<JsonField("safety_alerts_channel_id")>]
    SafetyAlertsChannelId: string option
}
