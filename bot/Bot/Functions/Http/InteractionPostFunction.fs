namespace SweeperSmackdown.Bot.Functions.Http

open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.Functions.Worker
open Microsoft.Extensions.Logging
open SweeperSmackdown.Bot.Discord
open SweeperSmackdown.Bot.Services
open System.IO

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
    ): IActionResult =
        // Get necessary contents of request and environment
        let signature = req.Headers["X-Signature-Ed25519"] |> Seq.head
        let timestamp = req.Headers["X-Signature-Timestamp"] |> Seq.head
        let body = (new StreamReader(req.Body)).ReadToEnd()

        let publicKey = configurationService.ReadOrThrow "DISCORD_PUBLIC_KEY"

        // Verify the request
        let verified = signingService.Verify(timestamp + body, signature, publicKey)
        
        // Return appropriate response
        if not verified then
            logger.LogInformation "Received an interaction without a valid signature"
            StatusCodeResult 401
        else
            logger.LogInformation $"Received an valid interaction of type {interaction.Type}"
            match interaction.Type with
            | InteractionType.PING -> OkObjectResult PingInteractionResponseDto
            | _ -> StatusCodeResult 500
