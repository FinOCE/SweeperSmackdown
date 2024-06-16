namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type AvatarDecorationData = {
    [<JsonField("asset")>]
    Asset: string

    [<JsonField("sku_id")>]
    SkuId: string
}
