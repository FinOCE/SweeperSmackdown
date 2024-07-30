namespace SweeperSmackdown.Bot.Requests

open FSharp.Json
open SweeperSmackdown.Bot.Types
open System.Net.Http

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
    static member Build(
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

    member this.SendAsync(channelId: string) = task {
        let client = new HttpClient()

        let req = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://discord.com/api/channels/{channelId}/invites",
            Content = new StringContent(Json.serialize this)
        )

        req.Headers.Clear()
        req.Headers.Add("Authorization", "Bearer ???") // TODO: Add bot token here

        let! res = client.SendAsync req
        let! body = res.Content.ReadAsStringAsync()
        return Json.deserialize<Invite> body
    }
