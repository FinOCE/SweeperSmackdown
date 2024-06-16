namespace SweeperSmackdown.Bot.Discord

open FSharp.Json
open System

type Entitlement = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("sku_id")>]
    SkuId: string

    [<JsonField("application_id")>]
    ApplicationId: string

    [<JsonField("user_id")>]
    UserId: string option

    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: EntitlementType

    [<JsonField("deleted")>]
    Deleted: bool

    [<JsonField("starts_at")>]
    StartsAt: DateTime option

    [<JsonField("ends_at")>]
    EndsAt: DateTime option

    [<JsonField("guild_id")>]
    GuildId: string option

    [<JsonField("consumed")>]
    Consumed: bool option
}
