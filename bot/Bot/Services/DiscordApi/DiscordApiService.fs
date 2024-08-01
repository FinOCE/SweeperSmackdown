namespace SweeperSmackdown.Bot.Services

open FSharp.Json
open System.Net.Http

type DiscordApiService (configurationService: IConfigurationService) =
    interface IDiscordApiService with
        member _.Send<'T> (endpoint: Endpoint<'T>, ?body: HttpContent) = task {
            let token = configurationService.GetValue "DISCORD_BOT_TOKEN"

            let client = new HttpClient()
            let req = new HttpRequestMessage(endpoint.Method, "https://discord.com/api/" + endpoint.Uri)

            if body.IsSome then
                req.Content <- body.Value

            req.Headers.Clear()
            req.Headers.Add("Authorization", $"Bearer {token}")

            let! res = client.SendAsync req
            let! body = res.Content.ReadAsStringAsync()
            return Json.deserialize<'T> body
        }
