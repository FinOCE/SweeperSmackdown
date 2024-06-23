namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

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
