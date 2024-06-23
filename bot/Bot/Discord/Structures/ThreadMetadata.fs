namespace SweeperSmackdown.Bot.Discord

open FSharp.Json
open System

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
