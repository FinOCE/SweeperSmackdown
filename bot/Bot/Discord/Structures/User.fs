namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type User = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("username")>]
    Username: string

    [<JsonField("discriminator")>]
    Discriminator: string

    [<JsonField("global_name")>]
    GlobalName: string option

    [<JsonField("avatar")>]
    Avatar: string option

    [<JsonField("bot")>]
    Bot: bool option

    [<JsonField("system")>]
    System: bool option

    [<JsonField("mfa_enabled")>]
    MfaEnabled: bool option

    [<JsonField("banner")>]
    Banner: string option

    [<JsonField("accent_color")>]
    AccentColor: int option

    [<JsonField("locale")>]
    Locale: string option

    [<JsonField("verified")>]
    Verified: bool option

    [<JsonField("email")>]
    Email: string option

    [<JsonField("flags")>]
    Flags: int option

    [<JsonField("premium_type", EnumValue = EnumMode.Value)>]
    PremiumType: UserPremiumType option

    [<JsonField("public_flags")>]
    PublicFlags: int option

    [<JsonField("avatar_decoration_data")>]
    AvatarDecorationData: AvatarDecorationData option
}
