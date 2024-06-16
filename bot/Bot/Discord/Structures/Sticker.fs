namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type Sticker = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("pack_id")>]
    PackId: string option

    [<JsonField("name")>]
    Name: string

    [<JsonField("description")>]
    Description: string option

    [<JsonField("tags")>]
    Tags: string

    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: StickerType

    [<JsonField("format_type", EnumValue = EnumMode.Value)>]
    FormatType: StickerFormatType

    [<JsonField("available")>]
    Available: bool option

    [<JsonField("guild_id")>]
    GuildId: string option

    [<JsonField("user")>]
    User: User option

    [<JsonField("sort_value")>]
    SortValue: int option
}
