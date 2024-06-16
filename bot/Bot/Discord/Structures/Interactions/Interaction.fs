namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type Interaction = {
    [<JsonField("id")>]
    Id: string

    [<JsonField("application_id")>]
    ApplicationId: string

    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: InteractionType

    [<JsonField("data")>]
    Data: InteractionData option

    [<JsonField("guild")>]
    Guild: Guild option

    [<JsonField("guild_id")>]
    GuildId: string option

    [<JsonField("channel")>]
    Channel: Channel option

    [<JsonField("channel_id")>]
    ChannelId: string option

    [<JsonField("member")>]
    Member: GuildMember option

    [<JsonField("user")>]
    User: User option

    [<JsonField("token")>]
    Token: string

    [<JsonField("version")>]
    Version: int

    [<JsonField("message")>]
    Message: Message option

    [<JsonField("app_permissions")>]
    AppPermissions: string

    [<JsonField("locale")>]
    Locale: string option

    [<JsonField("guild_locale")>]
    GuildLocale: string option

    [<JsonField("entitlements")>]
    Entitlements: Entitlement list

    [<JsonField("authorizing_integration_owners")>]
    AuthorizingIntegrationOwners: Map<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration>

    [<JsonField("context", EnumValue = EnumMode.Value)>]
    Context: InteractionContextType option
}
with
    static member Deserialize(json: string) =
        try Some(Json.deserialize<Interaction> json) with
        | _ -> None

    static member Deserialize<'T>(json: string) =
        try Some(Json.deserialize<'T> json) with
        | _ -> None

    static member GetInteractionType(json: string) =
        match Interaction.Deserialize<BaseInteraction> json with
        | Some interaction -> Some interaction.Type
        | None -> None
