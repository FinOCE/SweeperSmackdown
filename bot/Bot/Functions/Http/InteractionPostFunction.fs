namespace SweeperSmackdown.Bot.Functions.Http

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.Functions.Worker
open Microsoft.Extensions.Logging
open SweeperSmackdown.Bot.Discord
open SweeperSmackdown.Bot.Services
open System.IO
open System.Threading.Tasks

type InteractionPostFunction(
    logger: ILogger<InteractionPostFunction>,
    signingService: ISigningService,
    configurationService: IConfigurationService
) =
    [<Function(nameof InteractionPostFunction)>]
    member _.Run(
        [<HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "interaction")>] req: HttpRequest,
        executionContext: FunctionContext,
        [<FromBody>] interaction: Interaction
    ): Task<IActionResult> =
        task {
            match configurationService.TryGetValue "DISCORD_PUBLIC_KEY" with
            | None ->
                // Return error if variable missing
                logger.LogError "Missing environment variable DISCORD_PUBLIC_KEY"
                return StatusCodeResult 500 :> IActionResult
            | Some publicKey -> 
                // Get necessary contents of request and environment
                let signature = req.Headers["X-Signature-Ed25519"] |> Seq.tryHead
                let timestamp = req.Headers["X-Signature-Timestamp"] |> Seq.tryHead
                let! body = (new StreamReader(req.Body)).ReadToEndAsync()

                if Option.isNone signature || Option.isNone timestamp then
                    // Return error if missing headers
                    logger.LogInformation "Received an interaction without a signature or timestamp header"
                    return StatusCodeResult 401 :> IActionResult
                else
                    // Verify the request
                    let verified = signingService.Verify(timestamp.Value, body, signature.Value, publicKey)
        
                    // Return appropriate response
                    if not verified then
                        logger.LogInformation "Received an interaction without a valid signature"
                        return StatusCodeResult 401 :> IActionResult
                    else
                        logger.LogInformation $"Received an valid interaction of type {interaction.Type}"
                        match interaction.Type with
                        | InteractionType.PING -> return OkObjectResult PingInteractionResponseDto :> IActionResult
                        | _ -> return StatusCodeResult 500 :> IActionResult
        }
