namespace SweeperSmackdown.Bot.Functions.Http

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.Functions.Worker
open Org.BouncyCastle.Crypto.Signers
open SweeperSmackdown.Bot.Discord
open System
open System.IO
open System.Text
open Org.BouncyCastle.Crypto.Parameters

module InteractionPostFunction =
    [<Function("InteractionPostFunction")>]
    let Run
        ([<HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "interaction")>] req: HttpRequest)
        ([<FromBody>] interaction: Interaction)
        : IActionResult =
            // Get necessary contents of request and environment
            let signature = req.Headers["X-Signature-Ed25519"] |> Seq.head |> Convert.FromHexString
            let timestamp = req.Headers["X-Signature-Timestamp"] |> Seq.head

            let body = (new StreamReader(req.Body)).ReadToEnd()
            let message = Encoding.UTF8.GetBytes(timestamp + body)

            let publicKey = Environment.GetEnvironmentVariable("DISCORD_PUBLIC_KEY")

            // Verify the request
            let signer = Ed25519Signer()
            signer.Init(false, new Ed25519PublicKeyParameters(Encoding.UTF8.GetBytes(publicKey), 0))
            signer.BlockUpdate(message, 0, message.Length)
            let verified = signer.VerifySignature(signature)

            // Return appropriate response
            if not verified then
                StatusCodeResult(401)
            else
                match interaction.Type with
                | InteractionType.PING -> OkObjectResult(PingInteractionResponseDto())
                | _ -> StatusCodeResult(500)
