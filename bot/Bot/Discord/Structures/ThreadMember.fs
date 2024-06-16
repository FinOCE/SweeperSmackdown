namespace SweeperSmackdown.Bot.Discord

open FSharp.Json
open System

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
