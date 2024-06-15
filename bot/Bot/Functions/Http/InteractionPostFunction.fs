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
                return StatusCodeResult 500
            | Some publicKey -> 
                // Get necessary contents of request and environment
                let signature = req.Headers["X-Signature-Ed25519"] |> Seq.head
                let timestamp = req.Headers["X-Signature-Timestamp"] |> Seq.head
                let body = (new StreamReader(req.Body)).ReadToEnd()

                // Verify the request
                let verified = signingService.Verify(timestamp, body, signature, publicKey)
        
                // Return appropriate response
                if not verified then
                    logger.LogInformation "Received an interaction without a valid signature"
                    return StatusCodeResult 401
                else
                    logger.LogInformation $"Received an valid interaction of type {interaction.Type}"
                    match interaction.Type with
                    | InteractionType.PING -> return OkObjectResult PingInteractionResponseDto
                    | _ -> return StatusCodeResult 500
        }
