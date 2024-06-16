namespace SweeperSmackdown.Bot.Discord

open FSharp.Json
open System

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
