namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type MessageInteractionMetadata = {
    [<JsonField("id")>]
    Id: string
    
    [<JsonField("type")>]
    Type: InteractionType
    
    [<JsonField("user")>]
    User: User
    
    [<JsonField("authorizing_integration_owners")>]
    AuthorizingIntegrationOwners: Map<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration>

    [<JsonField("original_response_message_id")>]
    OriginalResponseMessage: string option

    [<JsonField("interacted_message_id")>]
    InteractedMessageId: string option

    [<JsonField("triggering_interaction_metadata")>]
    TriggeringInteractionMetadata: MessageInteractionMetadata option
}
