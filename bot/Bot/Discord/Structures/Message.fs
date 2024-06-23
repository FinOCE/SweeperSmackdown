namespace SweeperSmackdown.Bot.Discord

open FSharp.Json
open System

type MessageNonce =
    | Number of int
    | String of string

type Message = {
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
