namespace SweeperSmackdown.Bot.Discord

open FSharp.Json

type RoleSubscriptionData = {
    [<JsonField("role_subscription_listing_id")>]
    RoleSubscriptionListingId: string

    [<JsonField("tier_name")>]
    TierName: string

    [<JsonField("total_months_subscribed")>]
    TotalMonthsSubscribed: int

    [<JsonField("is_renewal")>]
    IsRenewal: bool
}
