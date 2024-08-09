namespace SweeperSmackdown.Bot.Functions.Http

open FSharp.Json
open Microsoft.Azure.Functions.Worker
open SweeperSmackdown.Bot.Types.Discord
open SweeperSmackdown.Bot.Services
open SweeperSmackdown.Bot.Commands
open System.IO
open System.Net
open Microsoft.Azure.Functions.Worker.Http

type InteractionPostFunction (
    configurationService: IConfigurationService,
    signingService: ISigningService,
    commandProvider: ICommandProvider
) =
    let verifySignature (headers: HttpHeadersCollection) (body: string) (publicKey: string) =
        let signatureExists = headers.Contains "X-Signature-Ed25519"
        let timestampExists = headers.Contains "X-Signature-Timestamp"

        if not signatureExists || not timestampExists then
            false
        else
            let signature = headers.GetValues "X-Signature-Ed25519" |> Seq.head
            let timestamp = headers.GetValues "X-Signature-Timestamp" |> Seq.head
            signingService.Verify(timestamp, body, signature, publicKey)

    [<Function(nameof InteractionPostFunction)>]
    member _.Run (
        [<HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "interaction")>] req: HttpRequestData
    ) = task {
        let publicKey = configurationService.GetValue "DISCORD_PUBLIC_KEY"
        let! body = (new StreamReader(req.Body)).ReadToEndAsync()

        match verifySignature req.Headers body publicKey with
        | false -> return req.CreateResponse HttpStatusCode.Unauthorized
        | true ->
            match Interaction.Deserialize body with
            // Invalid interaction
            | None -> return req.CreateResponse HttpStatusCode.BadRequest
            // Ping interaction
            | Some interaction when interaction.Type = InteractionType.PING ->
                let res = req.CreateResponse HttpStatusCode.OK
                res.Headers.Add("Content-Type", "application/json")
                do! InteractionCallback.build InteractionCallbackType.PONG
                    |> Json.serialize
                    |> res.WriteStringAsync
                return res
            // Application command interaction
            | Some interaction when interaction.Type = InteractionType.APPLICATION_COMMAND ->
                let! execution = commandProvider.Execute interaction
                match execution with
                | Error _ -> return req.CreateResponse HttpStatusCode.InternalServerError
                | Ok callback -> 
                    let res = req.CreateResponse HttpStatusCode.OK
                    res.Headers.Add("Content-Type", "application/json")
                    do! callback
                        |> Json.serialize
                        |> res.WriteStringAsync
                    return res
            // Unknown interaction type
            | _ ->
                return req.CreateResponse HttpStatusCode.InternalServerError
    }
