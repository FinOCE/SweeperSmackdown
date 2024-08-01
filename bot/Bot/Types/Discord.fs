namespace SweeperSmackdown.Bot.Types

open FSharp.Json
open System

// FS0049: Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open
//         declaration or a misspelt pattern name.
#nowarn "49"


type TextInputStyle =
    | SHORT = 1
    | PARAGRAPH = 2

type ButtonStyle =
    | PRIMARY = 1
    | SECONDARY = 2
    | SUCCESS = 3
    | DANGER = 4
    | LINK = 5

type MessageComponentType =
    | ACTION_ROW = 1
    | BUTTON = 2
    | STRING_SELECT = 3
    | TEXT_INPUT = 4
    | USER_SELECT = 5
    | ROLE_SELECT = 6
    | MENTIONABLE_SELECT = 7
    | CHANNEL_SELECT = 8

type PermissionOverwriteType =
    | ROLE = 0
    | MEMBER = 1

type ChannelForumLayout =
    | NOT_SET = 0
    | LIST_VIEW = 1
    | GALLERY_VIEW = 2

type ChannelSortOrder =
    | LATEST_ACTIVITY = 0
    | CREATION_DATE = 1

type VideoQualityMode =
    | AUTO = 1
    | FULL = 2

type PollLayoutType =
    | DEFAULT = 1

type TeamMembershipState =
    | INVITED = 1
    | ACCEPTED = 2

type MessageActivityType =
    | JOIN = 1
    | SPECTATE = 2
    | LISTEN = 3
    | JOIN_REQUEST = 5

type MessageType =
    | DEFAULT = 0
    | RECIPIENT_ADD = 1
    | RECIPIENT_REMOVE = 2
    | CALL = 3
    | CHANNEL_NAME_CHANGE = 4
    | CHANNEL_ICON_CHANGE = 5
    | CHANNEL_PINNED_MESSAGE = 6
    | USER_JOIN = 7
    | GUILD_BOOST = 8
    | GUILD_BOOST_TIER_1 = 9
    | GUILD_BOOST_TIER_2 = 10
    | GUILD_BOOST_TIER_3 = 11
    | CHANNEL_FOLLOW_ADD = 12
    | GUILD_DISCOVERY_DISQUALIFIED = 14
    | GUILD_DISCOVERY_REQUALIFIED = 15
    | GUILD_DISCOVERY_GRACE_PERIOD_INITIAL_WARNING = 16
    | GUILD_DISCOVERY_GRACE_PERIOD_FINAL_WARNING = 17
    | THREAD_CREATED = 18
    | REPLY = 19
    | CHAT_INPUT_COMMAND = 20
    | THREAD_STARTER_MESSAGE = 21
    | GUILD_INVITE_REMINDER = 22
    | CONTEXT_MENU_COMMAND = 23
    | AUTO_MODERATION_ACTION = 24
    | ROLE_SUBSCRIPTION_PURCHASE = 25
    | INTERACTION_PREMIUM_UPSELL = 26
    | STAGE_START = 27
    | STAGE_END = 28
    | STAGE_SPEAKER = 29
    | STAGE_TOPIC = 31
    | GUILD_APPLICATION_PREMIUM_SUBSCRIPTION = 32
    | GUILD_INCIDENT_ALERT_MODE_ENABLED = 36
    | GUILD_INCIDENT_ALERT_MODE_DISABLED = 37
    | GUILD_INCIDENT_REPORT_RAID = 38
    | GUILD_INCIDENT_REPORT_FALSE_ALARM = 39
    | PURCHASE_NOTIFICATION = 44

type ChannelType =
    | GUILD_TEXT = 0
    | DM = 1
    | GUILD_VOICE = 2
    | GROUP_DM = 3
    | GUILD_CATEGORY = 4
    | GUILD_ANNOUNCEMENT = 5
    | ANNOUNCEMENT_THREAD = 10
    | PUBLIC_THREAD = 11
    | PRIVATE_THREAD = 12
    | GUILD_STAGE_VOICE = 13
    | GUILD_DIRECTORY = 14
    | GUILD_FORUM = 15
    | GUILD_MEDIA = 16

type EntitlementType =
    | PURCHASE = 1
    | PREMIUM_SUBSCRIPTION = 2
    | DEVELOPER_GIFT = 3
    | TEST_MODE_PURCHASE = 4
    | FREE_PURCHASE = 5
    | USER_GIFT = 6
    | PREMIUM_PURCHASE = 7
    | APPLICATION_SUBSCRIPTION = 8

type UserPremiumType =
    | NONE = 0
    | NITRO_CLASSIC = 1
    | NITRO = 2
    | NITRO_BASIC = 3

type StickerFormatType = 
    | PNG = 1
    | APNG = 2
    | LOTTIE = 3
    | GIF = 4

type StickerType = 
    | STANDARD = 1
    | GUILD = 2

type GuildNsfwLevel =
    | DEFAULT = 0
    | EXPLICIT = 1
    | SAFE = 2
    | AGE_RESTRICTED = 3

type GuildPremiumTier =
    | NONE = 0
    | LEVEL_1 = 1
    | LEVEL_2 = 2
    | LEVEL_3 = 3

type GuildMfaLevel =
    | NONE = 0
    | ELEVATED = 1

type GuildExplicitContentFilterLevel =
    | DISABLED = 0
    | MEMBERS_WITHOUT_ROLES = 1
    | ALL_MEMBERS = 2

type GuildMessageNotificationLevel =
    | ALL_MESSAGES = 0
    | ONLY_MENTIONS = 1

type GuildVerificationLevel =
    | NONE = 0
    | LOW = 1
    | MEDIUM = 2
    | HIGH = 3
    | VERY_HIGH = 4

type CommandInteractionDataOptionValue =
    | String of string
    | Int of int
    | Double of double
    | Bool of bool

type ApplicationCommandType = 
    | CHAT_INPUT = 1
    | USER = 2
    | MESSAGE = 3

type ApplicationCommandOptionType =
    | SUB_COMMAND = 1
    | SUB_COMMAND_GROUP = 2
    | STRING = 3
    | INTEGER = 4
    | BOOLEAN = 5
    | USER = 6
    | CHANNEL = 7
    | ROLE = 8
    | MENTIONABLE = 9
    | NUMBER = 10
    | ATTACHMENT = 11

type InteractionContextType =
    | GUILD = 0
    | BOT_DM = 1
    | PRIVATE_CHANNEL = 2

type ApplicationIntegrationType =
    | GUILD_INSTALL = 0
    | USER_INSTALL = 1

type InteractionType = 
    | PING = 1
    | APPLICATION_COMMAND = 2
    | MESSAGE_COMPONENT = 3
    | APPLICATION_COMMAND_AUTOCOMPLETE = 4
    | MODAL_SUBMIT = 5

type InteractionCallbackType = 
    | PONG = 1
    | CHANNEL_MESSAGE_WITH_SOURCE = 4
    | DEFERRED_CHANNEL_MESSAGE_WITH_SOURCE = 5
    | DEFERRED_UPDATE_MESSAGE = 6
    | UPDATE_MESSAGE = 7
    | APPLICATION_COMMAND_AUTOCOMPLETE_RESULT = 8
    | MODAL = 9
    | PREMIUM_REQUIRED = 10

type InviteType =
    | GUILD
    | GROUP_DM
    | FRIEND

type InviteTargetType =
    | STREAM
    | EMBEDDED_APPLICATION

type DefaultReaction = {
    [<JsonField("emoji_id")>]
    EmojiId: string option
    
    [<JsonField("emoji_name")>]
    EmojiName: string option
}

type WelcomeScreenChannel = {
    [<JsonField("channel_id")>]
    ChannelId: string

    [<JsonField("description")>]
    Description: string
    
    [<JsonField("emoji_id")>]
    EmojiId: string option

    [<JsonField("emoji_name")>]
    EmojiName: string option
}

type WelcomeScreen = {
    [<JsonField("description")>]
    Description: string option

    [<JsonField("welcome_channels")>]
    WelcomeChannels: WelcomeScreenChannel list
}

type CommandInteractionDataOption = {
    [<JsonField("name")>]
    Name: string
    
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: ApplicationCommandOptionType
    
    [<JsonField("value")>]
    Value: CommandInteractionDataOptionValue option
    
    [<JsonField("options")>]
    Options: CommandInteractionDataOption list option

    [<JsonField("focused")>]
    Focused: bool option
}

type Attachment = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("filename")>]
    Filename: string

    [<JsonField("description")>]
    Description: string

    [<JsonField("content_type")>]
    ContentType: string option

    [<JsonField("size")>]
    Size: int

    [<JsonField("url")>]
    Url: string

    [<JsonField("proxy_url")>]
    ProxyUrl: string

    [<JsonField("height")>]
    Height: int option

    [<JsonField("width")>]
    Width: int option

    [<JsonField("ephemeral")>]
    Ephemeral: bool option

    [<JsonField("duration_secs")>]
    DurationSecs: float option

    [<JsonField("waveform")>]
    Waveform: string option

    [<JsonField("flags")>]
    Flags: int option
}

type RoleTags = {
    [<JsonField("bot_id")>]
    BotId: string option
    
    [<JsonField("integration_id")>]
    IntegrationId: string option

    [<JsonField("premium_subscriber")>]
    PremiumSubscriber: unit option

    [<JsonField("subscription_listing_id")>]
    SubscriptionListingId: string option
    
    [<JsonField("available_for_purchase")>]
    AvailableForPurchase: unit option
    
    [<JsonField("guild_connections")>]
    GuildConnections: unit option
}

// TODO: Check that `unit option` works as intended above. null = true, undefined = false

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

type Entitlement = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("sku_id")>]
    SkuId: string

    [<JsonField("application_id")>]
    ApplicationId: string

    [<JsonField("user_id")>]
    UserId: string option

    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: EntitlementType

    [<JsonField("deleted")>]
    Deleted: bool

    [<JsonField("starts_at")>]
    StartsAt: DateTime option

    [<JsonField("ends_at")>]
    EndsAt: DateTime option

    [<JsonField("guild_id")>]
    GuildId: string option

    [<JsonField("consumed")>]
    Consumed: bool option
}

type AvatarDecorationData = {
    [<JsonField("asset")>]
    Asset: string

    [<JsonField("sku_id")>]
    SkuId: string
}

type User = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("username")>]
    Username: string

    [<JsonField("discriminator")>]
    Discriminator: string

    [<JsonField("global_name")>]
    GlobalName: string option

    [<JsonField("avatar")>]
    Avatar: string option

    [<JsonField("bot")>]
    Bot: bool option

    [<JsonField("system")>]
    System: bool option

    [<JsonField("mfa_enabled")>]
    MfaEnabled: bool option

    [<JsonField("banner")>]
    Banner: string option

    [<JsonField("accent_color")>]
    AccentColor: int option

    [<JsonField("locale")>]
    Locale: string option

    [<JsonField("verified")>]
    Verified: bool option

    [<JsonField("email")>]
    Email: string option

    [<JsonField("flags")>]
    Flags: int option

    [<JsonField("premium_type", EnumValue = EnumMode.Value)>]
    PremiumType: UserPremiumType option

    [<JsonField("public_flags")>]
    PublicFlags: int option

    [<JsonField("avatar_decoration_data")>]
    AvatarDecorationData: AvatarDecorationData option
}

type GuildMember = {
    [<JsonField("user")>]
    User: User option
    
    [<JsonField("nick")>]
    Nick: string option

    [<JsonField("avatar")>]
    Avatar: string option

    [<JsonField("roles")>]
    Roles: string list

    [<JsonField("joined_at")>]
    JoinedAt: DateTime option

    [<JsonField("premium_since")>]
    PremiumSince: DateTime option

    [<JsonField("deaf")>]
    Deaf: bool

    [<JsonField("mute")>]
    Mute: bool

    [<JsonField("flags")>]
    Flags: int

    [<JsonField("pending")>]
    Pending: bool option

    [<JsonField("permissions")>]
    Permissions: string option

    [<JsonField("communication_disabled_until")>]
    CommunicationDisabledUntil: DateTime option

    [<JsonField("avatar_decoration_metadata")>]
    AvatarDecorationData: AvatarDecorationData option
}

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

type Sticker = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("pack_id")>]
    PackId: string option

    [<JsonField("name")>]
    Name: string

    [<JsonField("description")>]
    Description: string option

    [<JsonField("tags")>]
    Tags: string

    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: StickerType

    [<JsonField("format_type", EnumValue = EnumMode.Value)>]
    FormatType: StickerFormatType

    [<JsonField("available")>]
    Available: bool option

    [<JsonField("guild_id")>]
    GuildId: string option

    [<JsonField("user")>]
    User: User option

    [<JsonField("sort_value")>]
    SortValue: int option
}

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

type ChannelMention = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("guild_id")>]
    GuildId: string
    
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: ChannelType
    
    [<JsonField("name")>]
    Name: string
}

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

type ThreadMetadata = {
    [<JsonField("archived")>]
    Archived: bool
    
    [<JsonField("auto_archive_duration")>]
    AutoArchiveDuration: int
    
    [<JsonField("archive_timestamp")>]
    ArchiveTimestamp: DateTime
    
    [<JsonField("locked")>]
    Locked: bool
    
    [<JsonField("invitable")>]
    Invitable: bool option
    
    [<JsonField("create_timestamp")>]
    CreateTimestamp: DateTime option
}

type ThreadMember = {
    [<JsonField("id")>]
    Id: string option
    
    [<JsonField("user_id")>]
    UserId: string option
    
    [<JsonField("join_timestamp")>]
    JoinTimestamp: DateTime
    
    [<JsonField("flags")>]
    Flags: int
    
    [<JsonField("member")>]
    Member: GuildMember option
}

type ChannelTag = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("name")>]
    Name: string
    
    [<JsonField("moderated")>]
    Moderated: bool
    
    [<JsonField("emoji_id")>]
    EmojiId: string option
    
    [<JsonField("emoji_name")>]
    EmojiName: string option
}

type EmbedFooter = {
    [<JsonField("text")>]
    Text: string
    
    [<JsonField("icon_url")>]
    IconUrl: string option
    
    [<JsonField("proxy_icon_url")>]
    ProxyIconUrl: string option
}

type EmbedImage = {
    [<JsonField("url")>]
    Url: string
    
    [<JsonField("proxy_url")>]
    ProxyUrl: string option
    
    [<JsonField("height")>]
    Height: int option
    
    [<JsonField("width")>]
    Width: int option
}

type EmbedThumbnail = {
    [<JsonField("url")>]
    Url: string
    
    [<JsonField("proxy_url")>]
    ProxyUrl: string option
    
    [<JsonField("height")>]
    Height: int option
    
    [<JsonField("width")>]
    Width: int option
}

type EmbedVideo = {
    [<JsonField("url")>]
    Url: string option
    
    [<JsonField("proxy_url")>]
    ProxyUrl: string option
    
    [<JsonField("height")>]
    Height: int option
    
    [<JsonField("width")>]
    Width: int option
}

type EmbedProvider = {
    [<JsonField("name")>]
    Name: string option

    [<JsonField("url")>]
    Url: string option
}

type EmbedAuthor = {
    [<JsonField("name")>]
    Name: string

    [<JsonField("url")>]
    Url: string option

    [<JsonField("icon_url")>]
    IconUrl: string option
    
    [<JsonField("proxy_icon_url")>]
    ProxyIconUrl: string option
}

type EmbedField = {
    [<JsonField("name")>]
    Name: string

    [<JsonField("value")>]
    Value: string

    [<JsonField("inline")>]
    Inline: bool option
}

type Embed = {
    [<JsonField("title")>]
    Title: string option
    
    [<JsonField("type")>]
    Type: string option
    
    [<JsonField("description")>]
    Description: string option

    [<JsonField("url")>]
    Url: string option

    [<JsonField("timestamp")>]
    Timestamp: DateTime option

    [<JsonField("color")>]
    Color: int option

    [<JsonField("footer")>]
    Footer: EmbedFooter option

    [<JsonField("image")>]
    Image: EmbedImage option

    [<JsonField("thumbnail")>]
    Thumbnail: EmbedThumbnail option

    [<JsonField("video")>]
    Video: EmbedVideo option

    [<JsonField("provider")>]
    Provider: EmbedProvider option

    [<JsonField("author")>]
    Author: EmbedAuthor option

    [<JsonField("fields")>]
    Fields: EmbedField list option
}

type ReactionCountDetails = {
    [<JsonField("burst")>]
    Burst: int

    [<JsonField("normal")>]
    Normal: int
}

type Reaction = {
    [<JsonField("count")>]
    Count: int
    
    [<JsonField("count_details")>]
    CountDetails: ReactionCountDetails

    [<JsonField("me")>]
    Me: bool

    [<JsonField("me_burst")>]
    MeBurst: bool

    [<JsonField("emoji")>]
    Emoji: Emoji

    [<JsonField("burst_colors")>]
    BurstColors: int list
}

type MessageActivity = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: MessageActivityType

    [<JsonField("party_id")>]
    PartyId: string option
}

type OAuth2InstallParams = {
    [<JsonField("scopes")>]
    Scopes: string list

    [<JsonField("permissions")>]
    Permissions: string
}

type ApplicationIntegrationTypeConfiguration = {
    [<JsonField("oauth2_install_params")>]
    Oauth2InstallParams: OAuth2InstallParams option
}

type TeamMember = {
    [<JsonField("membership_state", EnumValue = EnumMode.Value)>]
    MembershipState: TeamMembershipState
    
    [<JsonField("team_id")>]
    TeamId: string

    [<JsonField("user")>]
    User: User

    [<JsonField("role")>]
    Role: string
}

type Team = {
    [<JsonField("icon")>]
    Icon: string option
    
    [<JsonField("id")>]
    Id: string

    [<JsonField("members")>]
    Members: TeamMember list

    [<JsonField("name")>]
    Name: string

    [<JsonField("owner_user_id")>]
    OwnerUserId: string
}

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

type MessageReference = {
    [<JsonField("message_id")>]
    MessageId: string option

    [<JsonField("channel_id")>]
    ChannelId: string option

    [<JsonField("guild_id")>]
    GuildId: string option

    [<JsonField("fail_if_not_exists")>]
    FailIfNotExists: bool option
}

type MessageInteractionMetadata = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("type")>]
    Type: InteractionType
    
    [<JsonField("user")>]
    User: User
    
    [<JsonField("authorizing_integration_owners")>]
    AuthorizingIntegrationOwners: Map<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration>

    [<JsonField("original_response_message_id")>]
    OriginalResponseMessage: string option

    [<JsonField("interacted_message_id")>]
    InteractedMessageId: string option

    [<JsonField("triggering_interaction_metadata")>]
    TriggeringInteractionMetadata: MessageInteractionMetadata option
}

type MessageInteraction = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("type")>]
    Type: InteractionType

    [<JsonField("name")>]
    Name: string

    [<JsonField("user")>]
    User: User

    [<JsonField("member")>]
    Member: GuildMember option
}

type RoleSubscriptionData = {
    [<JsonField("role_subscription_listing_id")>]
    RoleSubscriptionListingId: string

    [<JsonField("tier_name")>]
    TierName: string

    [<JsonField("total_months_subscribed")>]
    TotalMonthsSubscribed: int

    [<JsonField("is_renewal")>]
    IsRenewal: bool
}

type PollMedia = {
    [<JsonField("text")>]
    Text: string option

    [<JsonField("emoji")>]
    Emoji: Emoji option
}

type PollAnswer = {
    [<JsonField("answer_id")>]
    AnswerId: int

    [<JsonField("poll_media")>]
    PollMedia: PollMedia
}

type PollAnswerCount = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("count")>]
    Count: int

    [<JsonField("me_voted")>]
    MeVoted: bool
}

type PollResults = {
    [<JsonField("is_finalized")>]
    IsFinalized: bool

    [<JsonField("answer_counts")>]
    AnswerCounts: PollAnswerCount list
}

type Poll = {
    [<JsonField("question")>]
    Question: PollMedia

    [<JsonField("answers")>]
    Answers: PollAnswer list

    [<JsonField("expiry")>]
    Expiry: DateTime option

    [<JsonField("allow_multiselect")>]
    AllowMultiselect: bool

    [<JsonField("layout_type", EnumValue = EnumMode.Value)>]
    LayoutType: PollLayoutType

    [<JsonField("results")>]
    Results: PollResults option
}

type MessageCall = {
    [<JsonField("participants")>]
    Participants: string list

    [<JsonField("ended_timestamp")>]
    EndedTimestamp: DateTime option
}

type SelectMenuOption = {
    [<JsonField("label")>]
    Label: string

    [<JsonField("value")>]
    Value: string

    [<JsonField("description")>]
    Description: string option

    [<JsonField("emoji")>]
    Emoji: Emoji option

    [<JsonField("default")>]
    Default: bool option
}

type SelectMenuDefaultValue = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("type")>]
    Type: string
}

type BaseMessageComponent = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: MessageComponentType
}

and ActionRowComponent = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: MessageComponentType
    
    [<JsonField("components")>]
    Components: MessageComponent list
}

and ButtonComponent = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: MessageComponentType

    [<JsonField("style", EnumValue = EnumMode.Value)>]
    Style: ButtonStyle

    [<JsonField("label")>]
    Label: string

    [<JsonField("emoji")>]
    Emoji: Emoji option
    
    [<JsonField("custom_id")>]
    CustomId: string option

    [<JsonField("url")>]
    Url: string option

    [<JsonField("disabled")>]
    Disabled: bool option
}

and SelectMenuComponent = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: MessageComponentType

    [<JsonField("custom_id")>]
    CustomId: string

    [<JsonField("options")>]
    Options: SelectMenuOption list option

    [<JsonField("channel_types", EnumValue = EnumMode.Value)>]
    ChannelTypes: ChannelType list option

    [<JsonField("placeholder")>]
    Placeholder: string option

    [<JsonField("default_values")>]
    DefaultValues: SelectMenuDefaultValue option

    [<JsonField("min_values")>]
    MinValues: int option

    [<JsonField("max_values")>]
    MaxValues: int option

    [<JsonField("disabled")>]
    Disabled: bool option
}

and TextInputComponent = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: MessageComponentType

    [<JsonField("custom_id")>]
    CustomId: string

    [<JsonField("style", EnumValue = EnumMode.Value)>]
    Style: TextInputStyle

    [<JsonField("label")>]
    Label: string

    [<JsonField("min_length")>]
    MinLength: int option

    [<JsonField("max_length")>]
    MaxLength: int option

    [<JsonField("required")>]
    Required: bool option

    [<JsonField("value")>]
    Value: string option

    [<JsonField("placeholder")>]
    Placeholder: string option
}

and MessageComponent =
    | BaseMessageComponent

type MessageNonce =
    | Number of int
    | String of string

type Channel = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: ChannelType
    
    [<JsonField("guild_id")>]
    GuildId: string option
    
    [<JsonField("position")>]
    Position: int option
    
    [<JsonField("permission_overwrites")>]
    PermissionOverwrites: PermissionOverwrite list option

    [<JsonField("name")>]
    Name: string option
    
    [<JsonField("topic")>]
    Topic: string option
    
    [<JsonField("nsfw")>]
    Nsfw: bool option
    
    [<JsonField("last_message_id")>]
    LastMessageId: string option
    
    [<JsonField("bitrate")>]
    Bitrate: int option
    
    [<JsonField("user_limit")>]
    UserLimit: int option
    
    [<JsonField("rate_limit_per_user")>]
    RateLimitPerUser: int option
    
    [<JsonField("recipients")>]
    Recipients: User list option
    
    [<JsonField("icon")>]
    Icon: string option
    
    [<JsonField("owner_id")>]
    OwnerId: string option
    
    [<JsonField("application_id")>]
    ApplicationId: string option
    
    [<JsonField("managed")>]
    Managed: bool option
    
    [<JsonField("parent_id")>]
    ParentId: string option
    
    [<JsonField("last_pin_timestamp")>]
    LastPinTimestamp: DateTime option
    
    [<JsonField("rtc_region")>]
    RtcRegion: string option
    
    [<JsonField("video_quality_mode", EnumValue = EnumMode.Value)>]
    VideoQualityMode: VideoQualityMode option
    
    [<JsonField("message_count")>]
    MessageCount: int option
    
    [<JsonField("member_count")>]
    MemberCount: int option
    
    [<JsonField("thread_metadata")>]
    ThreadMetadata: ThreadMetadata option
    
    [<JsonField("member")>]
    Member: ThreadMember option
    
    [<JsonField("default_auto_archive_duration")>]
    DefaultAutoArchiveDuration: int option
    
    [<JsonField("permissions")>]
    Permissions: string option
    
    [<JsonField("flags")>]
    Flags: int option
    
    [<JsonField("total_messages_sent")>]
    TotalMessagesSent: int option
    
    [<JsonField("available_tags")>]
    AvailableTags: ChannelTag list option
    
    [<JsonField("applied_tags")>]
    AppliedTags: int list option
    
    [<JsonField("default_reaction_emoji")>]
    DefaultReactionEmoji: DefaultReaction option
    
    [<JsonField("default_thread_rate_limit_per_user")>]
    DefaultThreadRateLimitPerUser: int option
    
    [<JsonField("default_sort_order", EnumValue = EnumMode.Value)>]
    DefaultSortOrder: ChannelSortOrder option
    
    [<JsonField("default_forum_layout", EnumValue = EnumMode.Value)>]
    DefaultForumLayout: ChannelForumLayout option
}

type ResolvedData = {
    [<JsonField("users")>]
    Users: Map<string, User> option
    
    [<JsonField("members")>]
    Members: Map<string, GuildMember> option
    
    [<JsonField("roles")>]
    Roles: Map<string, Role> option
    
    [<JsonField("channels")>]
    Channels: Map<string, Channel> option
    
    [<JsonField("messages")>]
    Messages: Map<string, Message> option
    
    [<JsonField("attachments")>]
    Attachments: Map<string, Attachment> option
}

and Message = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("channel_id")>]
    ChannelId: string

    [<JsonField("author")>]
    Author: User

    [<JsonField("content")>]
    Content: string
    
    [<JsonField("timestamp")>]
    Timestamp: DateTime
    
    [<JsonField("edited_timestamp")>]
    EditedTimestamp: DateTime option
    
    [<JsonField("tts")>]
    Tts: bool
    
    [<JsonField("mention_everyone")>]
    MentionEveryone: bool

    [<JsonField("mentions")>]
    Mentions: User list

    [<JsonField("mention_roles")>]
    MentionRoles: string list

    [<JsonField("mention_channels")>]
    MentionChannels: ChannelMention list

    [<JsonField("attachments")>]
    Attachments: Attachment list

    [<JsonField("embeds")>]
    Embeds: Embed list

    [<JsonField("reactions")>]
    Reactions: Reaction list

    [<JsonField("nonce")>]
    Nonce: MessageNonce option

    [<JsonField("pinned")>]
    Pinned: bool

    [<JsonField("webhook_id")>]
    WebhookId: string option

    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: MessageType

    [<JsonField("activity")>]
    Activity: MessageActivity option

    [<JsonField("application")>]
    Application: Application option

    [<JsonField("message_reference")>]
    MessageReference: MessageReference option

    [<JsonField("flags")>]
    Flags: int

    [<JsonField("referenced_message")>]
    ReferencedMessage: Message option

    [<JsonField("interaction_metadata")>]
    InteractionMetadata: MessageInteractionMetadata option

    [<JsonField("interaction")>]
    Interaction: MessageInteraction option

    [<JsonField("thread")>]
    Thread: Channel option

    [<JsonField("components")>]
    Components: MessageComponent list option

    [<JsonField("sticker_items")>]
    StickerItems: Sticker list option

    [<JsonField("position")>]
    Position: int option

    [<JsonField("role_subscription_data")>]
    RoleSubscriptionData: RoleSubscriptionData option

    [<JsonField("resolved")>]
    Resolved: ResolvedData option

    [<JsonField("poll")>]
    Poll: Poll option

    [<JsonField("call")>]
    Call: MessageCall option
}

type Invite = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: InviteType

    [<JsonField("code")>]
    Code: string
    
    [<JsonField("guild")>]
    Guild: Guild option
    
    [<JsonField("channel")>]
    Channel: Channel option
    
    [<JsonField("inviter")>]
    Inviter: User option
    
    [<JsonField("target_type", EnumValue = EnumMode.Value)>]
    TargetType: InviteTargetType option
    
    [<JsonField("target_user")>]
    TargetUser: User option
    
    [<JsonField("target_application")>]
    TargetApplication: Application option
    
    [<JsonField("approximate_presence_count")>]
    ApproximatePresenceCount: int option
    
    [<JsonField("approximate_member_count")>]
    ApproximateMemberCount: int option
    
    [<JsonField("expires_at")>]
    ExpiresAt: DateTime

    // TODO: Add guild_scheduled_event here (not deprecated?)
}

type InteractionData = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("name")>]
    Name: string
    
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: ApplicationCommandType
    
    [<JsonField("resolved")>]
    Resolved: ResolvedData option

    [<JsonField("options")>]
    Options: CommandInteractionDataOption list option
    
    [<JsonField("guild_id")>]
    GuildId: string option
    
    [<JsonField("target_it")>]
    TargetId: string option
}

type BaseInteraction = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: InteractionType
}

type Interaction = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("application_id")>]
    ApplicationId: string

    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: InteractionType

    [<JsonField("data")>]
    Data: InteractionData option

    [<JsonField("guild")>]
    Guild: Guild option

    [<JsonField("guild_id")>]
    GuildId: string option

    [<JsonField("channel")>]
    Channel: Channel option

    [<JsonField("channel_id")>]
    ChannelId: string option

    [<JsonField("member")>]
    Member: GuildMember option

    [<JsonField("user")>]
    User: User option

    [<JsonField("token")>]
    Token: string

    [<JsonField("version")>]
    Version: int

    [<JsonField("message")>]
    Message: Message option

    [<JsonField("app_permissions")>]
    AppPermissions: string

    [<JsonField("locale")>]
    Locale: string option

    [<JsonField("guild_locale")>]
    GuildLocale: string option

    [<JsonField("entitlements")>]
    Entitlements: Entitlement list

    [<JsonField("authorizing_integration_owners")>]
    AuthorizingIntegrationOwners: Map<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration>

    [<JsonField("context", EnumValue = EnumMode.Value)>]
    Context: InteractionContextType option
}
with
    static member Deserialize(json: string) =
        try Some(Json.deserialize<Interaction> json) with
        | _ -> None

    static member Deserialize<'T>(json: string) =
        try Some(Json.deserialize<'T> json) with
        | _ -> None

    static member GetInteractionType(json: string) =
        match Interaction.Deserialize<BaseInteraction> json with
        | Some interaction -> Some interaction.Type
        | None -> None

type PingInteraction = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: InteractionType
}

type PingInteractionResponse = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: InteractionCallbackType
}

type AllowedMentions = {
    [<JsonField("parse")>]
    Parse: string list // "roles" "users" "everyone"
    
    [<JsonField("roles")>]
    Roles: string list option
    
    [<JsonField("users")>]
    Users: string list option
    
    [<JsonField("replied_user")>]
    RepliedUser: bool option
}
with
    static member build(
        Parse: string list,
        ?Roles: string list,
        ?Users: string list,
        ?RepliedUser: bool
    ) = {
        Parse = Parse;
        Roles = Roles;
        Users = Users;
        RepliedUser = RepliedUser;
    }

type Choice = {
    // TODO
    Temp: bool
}

type InteractionCallbackData = {
    // TODO: Figure out how to make this one of the three below
    Temp: bool
}

type InteractionCallbackMessageData = {
    [<JsonField("tts")>]
    Tts: bool option
    
    [<JsonField("content")>]
    Content: string option
    
    [<JsonField("embeds")>]
    Embeds: Embed list option
    
    [<JsonField("allowed_mentions")>]
    AllowedMentions: AllowedMentions option
    
    [<JsonField("flags")>]
    Flags: int option
    
    [<JsonField("components")>]
    Components: BaseMessageComponent list option
    
    [<JsonField("attachments")>]
    Attachments: Attachment list option
    
    [<JsonField("poll")>]
    Poll: Poll option
}
with
    static member build(
        ?Tts: bool,
        ?Content: string,
        ?Embeds: Embed list,
        ?AllowedMentions: AllowedMentions,
        ?Flags: int,
        ?Components: BaseMessageComponent list,
        ?Attachments: Attachment list,
        ?Poll: Poll
    ) = {
        Tts = Tts;
        Content = Content;
        Embeds = Embeds;
        AllowedMentions = AllowedMentions;
        Flags = Flags;
        Components = Components;
        Attachments = Attachments;
        Poll = Poll;
    }

type InteractionCallbackAutocompleteData = {
    [<JsonField("choices")>]
    Choices: Choice list
}

type InteractionCallbackModalData = {
    // TODO
    Temp: bool
}

type InteractionCallback = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: InteractionCallbackType
    
    [<JsonField("data")>]
    Data: InteractionCallbackMessageData option
}
with
    static member build(
        Type: InteractionCallbackType,
        ?Data: InteractionCallbackMessageData
    ) = {
        Type = Type;
        Data = Data;
    }
