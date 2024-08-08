namespace SweeperSmackdown.Bot.Requests

open FSharp.Json
open SweeperSmackdown.Bot.Types
open System.Collections.Generic

#nowarn "49"

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
