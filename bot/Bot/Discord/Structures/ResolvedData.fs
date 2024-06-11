namespace SweeperSmackdown.Bot.Discord

open System.Text.Json.Serialization

type ResolvedData =
    [<JsonPropertyName("users")>]
    val Users: Map<string, User> option
    
    [<JsonPropertyName("members")>]
    val Members: Map<string, GuildMember> option
    
    [<JsonPropertyName("roles")>]
    val Roles: Map<string, Role> option
    
    [<JsonPropertyName("channels")>]
    val Channels: Map<string, Channel> option
    
    [<JsonPropertyName("messages")>]
    val Messages: Map<string, Message> option
    
    [<JsonPropertyName("attachments")>]
    val Attachments: Map<string, Attachment> option
