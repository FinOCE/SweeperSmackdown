namespace SweeperSmackdown.Bot.Services

open FSharp.Json
open SweeperSmackdown.Bot.Types
open SweeperSmackdown.Bot.Requests
open System.Net.Http

type DiscordApiService (configurationService: IConfigurationService) =
    member _.Send<'T> (method: HttpMethod) (endpoint: string) (body: HttpContent option) = task {
        let token = configurationService.GetValue "DISCORD_BOT_TOKEN"

        let client = new HttpClient()
        let req = new HttpRequestMessage(method, "https://discord.com/api/" + endpoint)
        
        if body.IsSome then
            req.Content <- body.Value

        req.Headers.Clear()
        req.Headers.Add("Authorization", $"Bearer {token}")

        let! res = client.SendAsync req
        let! body = res.Content.ReadAsStringAsync()
        return Json.deserialize<'T> body
    }

    member _.Content<'T> (payload: 'T) =
        Some (new StringContent (Json.serialize payload) :> HttpContent)

    interface IDiscordApiService with
        member this.CreateChannelInvite (channelId: string) (payload: CreateChannelInvite) =
            this.Send<Invite>
                HttpMethod.Post
                $"channels/{channelId}/invites"
                (this.Content payload)
                
