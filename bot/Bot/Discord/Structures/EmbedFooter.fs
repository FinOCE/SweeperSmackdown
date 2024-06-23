namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type EmbedFooter = {
    [<JsonField("text")>]
    Text: string
    
    [<JsonField("icon_url")>]
    IconUrl: string option
    
    [<JsonField("proxy_icon_url")>]
    ProxyIconUrl: string option
}
