namespace SweeperSmackdown.Bot.Types.Discord

open FSharp.Json
open System.Collections.Generic

#nowarn "49"

type CreateChannelInvite = {
    [<JsonField("max_age")>]
    MaxAge: int option
    
    [<JsonField("max_uses")>]
    MaxUses: int option
    
    [<JsonField("temporary")>]
    Temporary: bool option
    
    [<JsonField("unique")>]
    Unique: bool option
    
    [<JsonField("target_type", EnumValue = EnumMode.Value)>]
    TargetType: InviteTargetType option
    
    [<JsonField("target_user_id")>]
    TargetUserId: string option
    
    [<JsonField("target_application_id")>]
    TargetApplicationId: string option
}
with
    static member build(
        ?maxAge: int,
        ?maxUses: int,
        ?temporary: bool,
        ?unique: bool,
        ?targetType: InviteTargetType,
        ?targetUserId: string,
        ?targetApplicationId: string
    ) = {
        MaxAge = maxAge;
        MaxUses = maxUses;
        Temporary = temporary;
        Unique = unique;
        TargetType = targetType;
        TargetUserId = targetUserId;
        TargetApplicationId = targetApplicationId;
    }

type CreateGlobalApplicationCommand = {
    [<JsonField("name")>]
    Name: string
    
    [<JsonField("name_localizations")>]
    NameLocalizations: Dictionary<string, string> option
    
    [<JsonField("description")>]
    Description: string option
    
    [<JsonField("description_localizations")>]
    DescriptionLocalizations: Dictionary<string, string> option
    
    [<JsonField("options")>]
    Options: ApplicationCommandOption list option
    
    [<JsonField("default_member_permissions")>]
    DefaultMemberPermissions: string option
    
    [<JsonField("dm_permissions")>]
    DmPermissions: bool option
    
    [<JsonField("name")>]
    IntegrationTypes: ApplicationIntegrationType list option
    
    [<JsonField("contexts")>]
    Contexts: InteractionContextType list option
    
    [<JsonField("type")>]
    Type: ApplicationCommandType option
    
    [<JsonField("nsfw")>]
    Nsfw: bool option
}
with
    static member build(
        Name: string,
        ?NameLocalizations: Dictionary<string, string>,
        ?Description: string,
        ?DescriptionLocalizations: Dictionary<string, string>,
        ?Options: ApplicationCommandOption list,
        ?DefaultMemberPermissions: string,
        ?DmPermissions: bool,
        ?IntegrationTypes: ApplicationIntegrationType list,
        ?Contexts: InteractionContextType list,
        ?Type: ApplicationCommandType,
        ?Nsfw: bool
    ) = {
        Name = Name;
        NameLocalizations = NameLocalizations;
        Description = Description;
        DescriptionLocalizations = DescriptionLocalizations;
        Options = Options;
        DefaultMemberPermissions = DefaultMemberPermissions;
        DmPermissions = DmPermissions;
        IntegrationTypes = IntegrationTypes;
        Contexts = Contexts;
        Type = Type;
        Nsfw = Nsfw;
    }
