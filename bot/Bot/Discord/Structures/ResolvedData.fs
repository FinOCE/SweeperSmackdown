namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type ResolvedData = {
    [<JsonField("users")>]
    Users: Map<string, User> option
    
    [<JsonField("members")>]
    Members: Map<string, GuildMember> option
    
    [<JsonField("roles")>]
    Roles: Map<string, Role> option
    
    //[<JsonField("channels")>]
    //Channels: Map<string, Channel> option
    
    //[<JsonField("messages")>]
    //Messages: Map<string, Message> option
    
    [<JsonField("attachments")>]
    Attachments: Map<string, Attachment> option
}
