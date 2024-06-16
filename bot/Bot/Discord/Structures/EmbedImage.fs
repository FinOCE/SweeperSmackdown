namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

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
