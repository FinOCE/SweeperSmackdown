namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type BaseMessageComponent = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: MessageComponentType
}

and ActionRowComponent = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: MessageComponentType
    
    [<JsonField("components")>]
    Components: MessageComponent list
}

and ButtonComponent = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: MessageComponentType

    [<JsonField("style", EnumValue = EnumMode.Value)>]
    Style: ButtonStyle

    [<JsonField("label")>]
    Label: string

    [<JsonField("emoji")>]
    Emoji: Emoji option
    
    [<JsonField("custom_id")>]
    CustomId: string option

    [<JsonField("url")>]
    Url: string option

    [<JsonField("disabled")>]
    Disabled: bool option
}

and SelectMenuComponent = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: MessageComponentType

    [<JsonField("custom_id")>]
    CustomId: string

    [<JsonField("options")>]
    Options: SelectMenuOption list option

    [<JsonField("channel_types", EnumValue = EnumMode.Value)>]
    ChannelTypes: ChannelType list option

    [<JsonField("placeholder")>]
    Placeholder: string option

    [<JsonField("default_values")>]
    DefaultValues: SelectMenuDefaultValue option

    [<JsonField("min_values")>]
    MinValues: int option

    [<JsonField("max_values")>]
    MaxValues: int option

    [<JsonField("disabled")>]
    Disabled: bool option
}

and TextInputComponent = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: MessageComponentType

    [<JsonField("custom_id")>]
    CustomId: string

    [<JsonField("style", EnumValue = EnumMode.Value)>]
    Style: TextInputStyle

    [<JsonField("label")>]
    Label: string

    [<JsonField("min_length")>]
    MinLength: int option

    [<JsonField("max_length")>]
    MaxLength: int option

    [<JsonField("required")>]
    Required: bool option

    [<JsonField("value")>]
    Value: string option

    [<JsonField("placeholder")>]
    Placeholder: string option
}

and MessageComponent =
    | BaseMessageComponent
    