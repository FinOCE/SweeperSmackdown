namespace SweeperSmackdown.Bot.Discord

open FSharp.Json
open System

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
