namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type RoleTags = {
    [<JsonField("bot_id")>]
    BotId: string option
    
    [<JsonField("integration_id")>]
    IntegrationId: string option

    [<JsonField("premium_subscriber")>]
    PremiumSubscriber: unit option

    [<JsonField("subscription_listing_id")>]
    SubscriptionListingId: string option
    
    [<JsonField("available_for_purchase")>]
    AvailableForPurchase: unit option
    
    [<JsonField("guild_connections")>]
    GuildConnections: unit option
}
