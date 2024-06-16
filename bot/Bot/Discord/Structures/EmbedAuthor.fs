namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type EmbedAuthor = {
    [<JsonField("name")>]
    Name: string

    [<JsonField("url")>]
    Url: string option

    [<JsonField("icon_url")>]
    IconUrl: string option
    
    [<JsonField("proxy_icon_url")>]
    ProxyIconUrl: string option
}
