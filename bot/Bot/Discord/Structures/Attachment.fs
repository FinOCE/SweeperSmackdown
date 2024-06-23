namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type Attachment = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("filename")>]
    Filename: string

    [<JsonField("description")>]
    Description: string

    [<JsonField("content_type")>]
    ContentType: string option

    [<JsonField("size")>]
    Size: int

    [<JsonField("url")>]
    Url: string

    [<JsonField("proxy_url")>]
    ProxyUrl: string

    [<JsonField("height")>]
    Height: int option

    [<JsonField("width")>]
    Width: int option

    [<JsonField("ephemeral")>]
    Ephemeral: bool option

    [<JsonField("duration_secs")>]
    DurationSecs: float option

    [<JsonField("waveform")>]
    Waveform: string option

    [<JsonField("flags")>]
    Flags: int option
}
