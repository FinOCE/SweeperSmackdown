namespace SweeperSmackdown.Bot.Functions.Http

open FSharp.Json
open Microsoft.Azure.Functions.Worker
open SweeperSmackdown.Bot.Discord
open SweeperSmackdown.Bot.Services
open System.IO
open System.Net
open System.Threading.Tasks
open Microsoft.Azure.Functions.Worker.Http

type TempRequestInteraction = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: InteractionType
}

type TempResponseInteraction = {
    [<JsonField("type", EnumValue = EnumMode.Value)>]
    Type: InteractionCallbackType
}

type InteractionPostFunction(
    signingService: ISigningService,
    configurationService: IConfigurationService
) =
    [<Function(nameof InteractionPostFunction)>]
    member _.Run(
        [<HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "interaction")>] req: HttpRequestData
    ): Task<HttpResponseData> =
        task {
            // Get the public key from the environment
            match configurationService.TryGetValue "DISCORD_PUBLIC_KEY" with
            | None -> return req.CreateResponse HttpStatusCode.InternalServerError
            | Some publicKey -> 

            // Get the body of the request
            let! body = (new StreamReader(req.Body)).ReadToEndAsync()
            let interactionOption = 
                try Some(Json.deserialize<TempRequestInteraction> body)
                with exn -> None

            match interactionOption with
            | None -> return req.CreateResponse HttpStatusCode.BadRequest
            | Some interaction ->

            // Get necessary contents of request and environment
            let signatureExists = req.Headers.Contains "X-Signature-Ed25519"
            let timestampExists = req.Headers.Contains "X-Signature-Timestamp"

            if not signatureExists || not timestampExists then
                return req.CreateResponse HttpStatusCode.Unauthorized
            else

            let signature = req.Headers.GetValues "X-Signature-Ed25519" |> Seq.head
            let timestamp = req.Headers.GetValues "X-Signature-Timestamp" |> Seq.head

            // Verify the request
            let verified = signingService.Verify(timestamp, body, signature, publicKey)
        
            if not verified then
                return req.CreateResponse HttpStatusCode.Unauthorized
            else

            // Return appropriate response
            match interaction.Type with
            | InteractionType.PING -> 
                let res = req.CreateResponse HttpStatusCode.OK
                res.Headers.Add("Content-Type", "application/json")
                let content: TempResponseInteraction = { Type = InteractionCallbackType.PONG }
                do! res.WriteStringAsync(Json.serialize content)
                return res
            | _ ->
                return req.CreateResponse HttpStatusCode.InternalServerError
        }
